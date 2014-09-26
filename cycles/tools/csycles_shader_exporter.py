#Copyright 2014 Robert McNeel and Associates
#
#Licensed under the Apache License, Version 2.0 (the "License");
#you may not use this file except in compliance with the License.
#You may obtain a copy of the License at
#
#http://www.apache.org/licenses/LICENSE-2.0
#
#Unless required by applicable law or agreed to in writing, software
#distributed under the License is distributed on an "AS IS" BASIS,
#WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
#See the License for the specific language governing permissions and
#limitations under the License.

import bpy
from bpy import data as D
from bpy import context as C

import functools

# contain all nodes for a material
allnodes = set()
# contain all links for a material
alllinks = set()

# Node type to CSycle class name
# TODO: add all nodes
nodemapping = {

# shader nodes
    'ADD_SHADER' : 'AddClosureNode',
    'MIX_SHADER' : 'MixClosureNode',
    
    'EMISSION' : 'EmissionNode',

    'BSDF_DIFFUSE' : 'DiffuseBsdfNode',
    'BSDF_GLOSSY' : 'GlossyBsdfNode',
    'BSDF_REFRACTION' : 'RefractionBsdfNode',

# texture nodes

    'TEX_IMAGE' : 'ImageTextureNode',
    'TEX_BRICK' : 'BrickTexture',
    'TEX_CHECKER' : 'CheckerTexture',
        
# color nodes
    'MIX_RGB' : 'MixNode',

# conversion nodes
    'MATH' : 'MathNode',
    'RGBTOBW' : 'RgbToBwNode',

# vector nodes
    'BUMP' : 'BumpNode',

# input nodes
    'VALUE' : 'ValueNode',
    'RGB' : 'ColorNode',
    'FRESNEL' : 'FresnelNode',
    'TEX_COORD' : 'TextureCoordinateNode',

# output nodes
    'BACKGROUND' : 'BackgroundNode',
}

# Socket type to CSycles class name mapping
socketmapping = {
    'SHADER' : 'ClosureSocket',
    'VALUE' : 'FloatSocket',
    'RGBA' : 'Float4Socket',
    'VECTOR' : 'Float4Socket',
}

class Link():
    """
    Class to denote a link between two nodes and sockets. We need this
    because we very likely need to simplify the graph by dropping unnecessary
    nodes like REROUTE and relinking nodes and sockets instead.
    """
    def __init__(self, fromnode, fromsock, tonode, tosock):
        self.fromnode = fromnode
        self.fromsock = fromsock
        self.tonode = tonode
        self.tosock = tosock
    
    def __str__(self):
        fromnode = self.fromnode.label if self.fromnode.label else self.fromnode.name
        tonode = self.tonode.label if self.tonode.label else self.tonode.name
        return '['+ fromnode + ']>' + self.fromsock.name + ' -> ' + \
                self.tosock.name + '('+self.tosock.type+')<[' + tonode + ']'
    
    def __repr__(self):
        return self.__str__()
    
    def __lt__(a, b):
        return str(a) < str(b)

def get_node_name(node):
    """
    Return a cleaned up node name, usable as variable
    """
    return cleanup_name(node.label if node.label else node.name)

def get_node_name_from_tuple(ntup):
    return get_node_name(ntup[0])

def cleanup_name(name):
    """
    Adapt name so it can be used as a variable name
    """
    name = name.replace(" ", "_")
    name = name.replace(".", "_")
    return name.lower()

def get_socket_name(socket, socketlist, is_input, node):
    """
    Determine name for a socket. The names given through BPy differ from
    what is at the lowest level. This level is also what C[CS]?ycles uses,
    so some mapping is necessary.
    """
    sockname = socket.name
    if is_input:
        if node.type == 'MATH':
            i = 0
            for sck in socketlist.items():
                i+=1
                if sck[1]==socket:
                    name = socket.name
                    sockname = name + str(i)
                    break
        if socket.name == "Shader":
            i = 0
            for sck in socketlist.items():
                if sck[1].name == "Shader": i+=1
                if sck[1]==socket:
                    sockname = "Closure" + str(i)
                    break
    else:
        if socket.name == "Shader":
            if 'BSDF' in node.type:
                sockname = "BSDF"
            else:
                sockname = "Closure"
    sockname = sockname.replace(" ", "")
    return sockname

def get_val_string(input):
    """
    String for a value, formatted for CSycles
    """
    val = ""
    if input.type == 'VALUE':
        val = "{0: .3f}f".format(input.default_value)
    elif input.type in ('RGBA', 'VECTOR'):
        val = "new float4({0: .3f}f, {1: .3f}f, {2: .3f}f)".format(
            input.default_value[0],
            input.default_value[1],
            input.default_value[2]
        )
    return val

def find_output_node_of_group(group):
    """
    Get the output node for a group.
    """
    for n in group.nodes:
        if n.type == 'GROUP_OUTPUT':
            return n

def find_actual_from_link(link, nt_and_parent, depth=0):
    """
    Find the node a link is coming from. Simplify by getting rid of
    REROUTE and group boundaries.
    """
    s = depth*"\t"
    
    nt = nt_and_parent[0]
    parentn = nt_and_parent[1]

    l = link

    # REROUTE node, just check it's input
    if link.from_node.type =='REROUTE':
        l = find_actual_from_link(link.from_node.inputs[0].links[0], nt_and_parent, depth+1)
    
    # GROUP node, find its GROUP_OUTPUT and find corresponding sockets
    # (names shall be equal)
    elif link.from_node.type == 'GROUP':
        ngroup = link.from_node
        outn = find_output_node_of_group(ngroup.node_tree)

        #now find matching sockets (group output socket vs outpnode input socket
        for inps in outn.inputs:

            # CUSTOM is a point where new sockets can be created in blender interface
            if inps.type=='CUSTOM':
                continue
            if inps.links[0].to_socket.name==link.from_socket.name:
                l = find_actual_from_link(inps.links[0], (nt, ngroup.node_tree), depth+1)
    elif link.from_node.type == 'GROUP_INPUT':
        for inp in parentn.inputs:
            if inp.name == link.from_socket.name:
                l = find_actual_from_link(inp.links[0], nt_and_parent, depth+1) # got a match, lets use that!

    return l

def find_output_node(nodetree):
    """
    Get first output node that has incoming links.
    """
    for n in nodetree.nodes:
        if n.type.startswith('OUTPUT'):
            for inp in n.inputs:
                if inp.is_linked:
                    return n
    return None

def find_connections(node_and_tree, depth = 0):
    """
    Iterate over nodes and find the links needed. This process
    will also minimize the graph by removing redundant nodes like REROUTE
    and simplify through removing of group boundaries.
    """
    node = node_and_tree[0]
    nt = node_and_tree[1]
    parentn = node_and_tree[2]
    for input in node.inputs:
        if input.is_linked:
            #shorthand to link
            lnk = input.links[0]
            
            # to and from
            tonode = lnk.to_node
            tosocket = lnk.to_socket
            fromnode = lnk.from_node
            fromsocket = lnk.from_socket
            
            l = None
            # we need to minimize if any of REROUTE, GROUP_OUTPUT, GROUP_INPUT
            # or GROUP. Find the node and socket we actually need to link from
            if fromnode.type in ('REROUTE', 'GROUP_OUTPUT', 'GROUP_INPUT', 'GROUP'):
                fromlnk = find_actual_from_link(input.links[0], (nt, parentn), depth)
                l = Link(fromlnk.from_node, fromlnk.from_socket, tonode, tosocket)
            else: # nothing special, we have the node and socket we need                
                l = Link(fromnode, fromsocket, tonode, tosocket)

            if l:
                alllinks.add(l)

def is_connected(node):
    return ([True for inp in node.inputs if inp.is_linked] +
            [True for outp in node.outputs if outp.is_linked]).count(True)>0
            
def add_nodes(nodeset, nt, parentn=None):
    """
    Add all useful nodes to a set.
    """
    for n in nt.nodes:
        if not n.type in ('FRAME','REROUTE', 'GROUP_INPUT', 'GROUP_OUTPUT'):
            if n.type == 'GROUP':
                add_nodes(nodeset, n.node_tree, n)
            else:
                #print("add_nodes: Adding", n.label if n.label else n.name, n.type)
                if is_connected(n):
                    nodeset.add((n, nt, parentn))

def add_inputs(varname, inputs):
    """
    Iterate over given inputs and create input socket value setters
    """
    inputinits = ""
    valnamecount = 1
    for inp in inputs:
        if inp.type == 'SHADER': continue
        if inp.is_linked: continue
    
        inpname = get_socket_name(inp, inputs, True, inp.node)
        inputinits = inputinits + "\t{0}.ins.{1}.Value = {2};\n".format(
            varname, inpname, get_val_string(inp)
        )
    
    return inputinits

def code_init_node(node):
    """
    Set node variables and sockets to their proper values
    """
    varname = get_node_name(node)
    initcode = ""
    initcode = add_inputs(varname, node.inputs)

    # now that inputs have been set, make sure we handle the
    # extra cases, like 'direct member' values as Color and Distribution, Value
    if node.type in ('BSDF_GLOSSY', 'BSDF_REFRACTION'):
        initcode = initcode + "\t{0}.Distribution = \"{1}\";\n".format(
            varname,
            node.distribution.lower().capitalize()
        )
    if node.type == 'RGB':
        initcode = initcode + "\t{0}.Value = new float4({1:.3f}f, {2:.3f}f, {3:.3f}f);\n".format(
            varname,
            node.color[0],
            node.color[1],
            node.color[2]
        )
    if node.type == 'VALUE':
        initcode = initcode + "\t{0}.Value = {1:.3f}f;\n".format(
            varname,
            node.outputs[0].default_value
        )
    if node.type == 'MATH':
        opval = functools.reduce(
            lambda x, y: x+'_'+y,
            map(str.capitalize, node.operation.lower().split('_'))
        )
        initcode = initcode + "\t{0}.Operation = MathNode.Operations.{1};\n".format(
            varname,
            opval
        )
    return initcode

def code_instantiate_nodes(nodes):
    """
    Construct CSycles nodes for each Blender cycles node,
    initialise and return as one code string.
    """
    code = ""
    for ntup in nodes:
        n = ntup[0]
        if 'OUTPUT' in n.type: continue
           
        nodeconstruct = "\tvar {0} = new {1}();\n".format(
            get_node_name(n), nodemapping[n.type])
        nodeinitialise = code_init_node(n)
        
        code = code + nodeconstruct + nodeinitialise + "\n"

    return code

def code_new_shader(shadername, shadertype):
    """Start code for a new shader."""
    return "\tvar {0} = new Shader(Client, Shader.ShaderType.{1});\n\n\t{0}.Name = \"{0}\";\n\t{0}.UseMis = false;\n\t{0}.UseTransparentShadow = true;\n\t{0}.HeterogeneousVolume = false;\n\n".format(shadername, shadertype)

def code_nodes_to_shader(shadername, nodes):
    """Create the code that adds node instances to the shader."""
    addcode = ""
    for ntup in nodes:
        n = ntup[0]
        
        if 'OUTPUT' in n.type: continue
    
        addcode = addcode + "\t{0}.AddNode({1});\n".format(shadername, get_node_name(n))
    
    return addcode

def code_finalise(shadername, links):
    """Find the links that go into the final output node."""
    finalisecode = ""
    for link in links:
        fromnode = link.fromnode
        fromsock = link.fromsock
        tonode = link.tonode
        tosock = link.tosock
        
        if not 'OUTPUT' in tonode.type: continue
    
        fromsockname = get_socket_name(fromsock, fromnode.inputs, False, fromnode)
        tosockname = get_socket_name(tosock, tonode.inputs, True, tonode)
        
        finalisecode = finalisecode + "\t{2}.outs.{3}.Connect({0}.Output.ins.{1});\n".format(
            shadername,
            tosockname,
            get_node_name(fromnode),
            fromsockname
        )
    finalisecode = finalisecode + "\n\t{0}.FinalizeGraph();\n".format(shadername)
    finalisecode = finalisecode + "\n\treturn {0};".format(shadername)
    return finalisecode

def code_link_nodes(links):
    """Add code to link nodes to each other."""
    linkcode = ""
    for link in links:
        fromnode = link.fromnode
        fromsock = link.fromsock
        tonode = link.tonode
        tosock = link.tosock
        
        if 'OUTPUT' in tonode.type: continue
        
        fromsockname = get_socket_name(fromsock, fromnode.outputs, False, fromnode)
        tosockname = get_socket_name(tosock, tonode.inputs, True, tonode)
        
        linkcode = linkcode + "\t{0}.outs.{1}.Connect({2}.ins.{3});\n".format(
            get_node_name(fromnode),
            fromsockname,
            get_node_name(tonode),
            tosockname
        )
    
    return linkcode        

def main():
    # do the material shaders
    for ob in C.selected_objects:
        nt = ob.material_slots[0].material.node_tree
        shadername = ob.material_slots[0].material.name
        shadername = cleanup_name(shadername)        
        create_shader(shadername, nt)
    # do the world shaders
    for world in D.worlds:
        nt = world.node_tree
        shadername = world.name
        shadername = cleanup_name(shadername)
        create_shader(shadername, nt, True)

def create_shader(shadername, nt, is_world=False):
        nodetree_stack = []
        allnodes.clear()
        alllinks.clear()
                
        ## seed our nodes set
        add_nodes(allnodes, nt)
        
        ## seed our links set
        for n in allnodes:
            find_connections(n)

        # make nice, sorted lists
        nodelist = list(allnodes)
        nodelist.sort(key=get_node_name_from_tuple)
        linklist = list(alllinks)
        linklist.sort()
        
        print(linklist)

        # creat new shader
        shadertype = "Background" if is_world else "Material"
        shadercode = code_new_shader(shadername, shadertype)
        
        # create node setup code        
        nodesetup = code_instantiate_nodes(nodelist)
        # add our nodes to shader
        nodeadd = code_nodes_to_shader(shadername, nodelist)
        # link our nodes in CSycles
        linksetup = code_link_nodes(linklist)
        # finalise everything
        finalisecode = code_finalise(shadername, linklist)
        
        csycles_shader_creation = "public Shader create_{0}_shader()\n{{\n{1}\n{2}\n{3}\n{4}\n{5}\n}}".format(
            shadername,
            shadercode,
            nodesetup,
            nodeadd,
            linksetup,
            finalisecode
        )
        
        exportfile = None
        exportfilename = "{0}.cs".format(shadername)
        for t in D.texts:
            if t.name == exportfilename:
                exportfile = t
        
        if not exportfile:
            exportfile = D.texts.new(exportfilename)
        
        exportfile.clear()
        exportfile.write(csycles_shader_creation)

main()
