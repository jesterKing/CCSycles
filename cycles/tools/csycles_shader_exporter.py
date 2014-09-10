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

# contain all nodes for a material
allnodes = set()
# contain all links for a material
alllinks = set()

# Node type to CSycle class name
nodemapping = {
    'ADD_SHADER' : 'AddClosureNode',
    'MIX_SHADER' : 'MixClosureNode',
    
    'MIX_RGB' : 'MixNode',

    'MATH' : 'MathNode',
    'VALUE' : 'ValueNode',
    'RGB' : 'ColorNode',
    'FRESNEL' : 'FresnelNode',
        
    'EMISSION' : 'EmissionNode',

    'BSDF_DIFFUSE' : 'DiffuseBsdfNode',
    'BSDF_GLOSSY' : 'GlossyBsdfNode',
    'BSDF_REFRACTION' : 'RefractionBsdfNode',
}

socketmapping = {
    'SHADER' : 'ClosureSocket',
    'VALUE' : 'FloatSocket',
    'RGBA' : 'Float4Socket',
    'VECTOR' : 'Float4Socket',
}

class Link():
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
    
    def __lt__(a, b):
        return str(a) < str(b)

def find_output_node_of_group(group):
    for n in group.nodes:
        if n.type == 'GROUP_OUTPUT':
            return n

def find_actual_from_link(link, nt_and_parent, depth=0):
    s = depth*"\t"
    
    nt = nt_and_parent[0]
    parentn = nt_and_parent[1]

    #print(s+"!! got link to " + link.to_node.name + "("+link.to_node.label+"):"+link.to_socket.name + " from " + link.from_node.name +"("+link.from_node.label+"):" + link.from_socket.name)
    l = link

    # REROUTE node, just check it's input
    if link.from_node.type =='REROUTE':
        #print("rerouting")
        l = find_actual_from_link(link.from_node.inputs[0].links[0], nt_and_parent, depth+1)
    
    # GROUP node, find its GROUP_OUTPUT and find corresponding sockets
    # (names shall be equal)
    elif link.from_node.type == 'GROUP':
        ngroup = link.from_node
        #print("find_actual_from_link: " +s + " >> linking from GROUP " + ngroup.name + "("+ngroup.label+") -- " + ngroup.node_tree.name )
        outn = find_output_node_of_group(ngroup.node_tree)

        #now find matching sockets (group output socket vs outpnode input socket
        for inps in outn.inputs:

            # CUSTOM is a point where new sockets can be created in blender interface
            if inps.type=='CUSTOM':
                continue
            if inps.links[0].to_socket.name==link.from_socket.name:
                #print("find_actual_from_link: " + s + " actual node to link from: " + inps.links[0].from_node.name + "(" + inps.links[0].from_node.label +")")
                l = find_actual_from_link(inps.links[0], (nt, ngroup.node_tree), depth+1)
    elif link.from_node.type == 'GROUP_INPUT':
        for inp in parentn.inputs:
            if inp.name == link.from_socket.name:
                l = find_actual_from_link(inp.links[0], nt_and_parent, depth+1) # got a match, lets use that!
    

    return l

def find_output_node(nodetree):
    for n in nodetree.nodes:
        if n.type.startswith('OUTPUT'):
            return n
    return None

def find_connections(node_and_tree, depth = 0):
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
                #print("find_connections: need to find actual node, minimizing...")
                fromlnk = find_actual_from_link(input.links[0], (nt, parentn), depth)
                l = Link(fromlnk.from_node, fromlnk.from_socket, tonode, tosocket)
            else: # nothing special, we have the node and socket we need                
                l = Link(fromnode, fromsocket, tonode, tosocket)

            if l:
                #print("find_connections: add link", l)
                alllinks.add(l)

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
                nodeset.add((n, nt, parentn))

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

def add_inputs(varname, inputs):
    """
    Iterate over given inputs and create input socket value setters
    """
    inputinits = ""
    valnamecount = 1
    for inp in inputs:
        if inp.type == 'SHADER': continue
    
        inpname = inp.name
        if inpname == "Value": # special case, for Math node we see Value1 and Value2, but we get only Value
            inpname = inpname + str(valnamecount)
            valnamecount = valnamecount + 1
        inputinits = inputinits + "{0}.ins.{1}.Value = {2};\n".format(
            varname, inpname, get_val_string(inp)
        )
    
    return inputinits

def initialise_node(node):
    """
    Set node variables and sockets to their proper values
    """
    varname = node.label if node.label else node.name
    initcode = ""
    initcode = add_inputs(varname, node.inputs)

    # now that inputs have been set, make sure we handle the
    # extra cases, like 'direct member' values as Color and Distribution, Value
    if node.type in ('BSDF_GLOSSY', 'BSDF_REFRACTION'):
        initcode = initcode + "{0}.Distribution = \"{1}\";\n".format(
            varname,
            node.distribution.lower().capitalize()
        )
    if node.type == 'RGB':
        initcode = initcode + "{0}.Value = new float4({1:.3f}f, {2:.3f}f, {3:.3f}f);\n".format(
            varname,
            node.color[0],
            node.color[1],
            node.color[2]
        )
    if node.type == 'VALUE':
        initcode = initcode + "{0}.Value = {1:.3f}f;\n".format(
            varname,
            node.outputs[0].default_value
        )
    if node.type == 'MATH':
        initcode = initcode + "{0}.Operation = {1};\n".format(
            varname,
            node.operation.lower().capitalize()
        )
    return initcode

def generate_node_code(nodes):
    """
    Construct CSycles nodes for each Blender cycles node,
    initialise and return as one code string.
    """
    code = ""
    for ntup in nodes:
        n = ntup[0]
        if 'OUTPUT' in n.type: continue
           
        nodeconstruct = "var {0} = new {1}();\n".format(
            n.label if n.label else n.name, nodemapping[n.type])
        nodeinitialise = initialise_node(n)
        
        code = code + nodeconstruct + nodeinitialise + "\n"

    return code

def add_nodes_to_shader(shadername, nodes):
    pass

def generate_linking_code(links):
    pass

def get_node_name(ntup):
    return ntup[0].label if ntup[0].label else ntup[0].name

def main():            
    for ob in C.selected_objects:
        nodetree_stack = []
        allnodes.clear()
        alllinks.clear()
        
        nt = ob.material_slots[0].material.node_tree
        shadername = ob.material_slots[0].material.name
        
        shadername = shadername.replace(" ", "_")
        shadername = shadername.replace(".", "_")
        
        ## seed our nodes set
        add_nodes(allnodes, nt)
        
        ## seed our links set
        for n in allnodes:
            find_connections(n)

        # make nice, sorted lists
        nodelist = list(allnodes)
        nodelist.sort(key=get_node_name)
        linklist = list(alllinks)
        linklist.sort()

        
        # create node setup code        
        nodesetup = generate_node_code(nodelist)
        # add our nodes to shader
        nodeadd = add_nodes_to_shader(shadername, nodes)
        # link our nodes in CSycles
        linksetup = generate_linking_code(linklist)
        
        print(nodesetup)
        print(linksetup)
        
main()
