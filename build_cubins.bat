SET nvcc=C:\CUDA\bin\nvcc.exe
SET cyclesroot=D:\Dev\CCSycles
SET cyclesout=D:\Dev\CCSycles
SET cudaversion=65
SET shadermodelnum=%1
SET shadermodel=sm_%shadermodelnum%

IF [%shadermodelnum%]==[] (
	FOR %%s IN ("sm_20", "sm_21", "sm_30", "sm_35", "sm_50", "sm_52") DO (
		"%nvcc%" -arch=%%s -m64 --cubin %cyclesroot%/cycles/src/kernel/kernels/cuda/kernel.cu -o %cyclesout%/lib/kernel_%%s.cubin --ptxas-options="-v" -D__KERNEL_CUDA_VERSION__=%cudaversion% --use_fast_math -I%cyclesroot%/cycles/src/kernel/../util -I%cyclesroot%/cycles/src/kernel/svm -DCCL_NAMESPACE_BEGIN= -DCCL_NAMESPACE_END= -DNVCC
	)
) ELSE (
	"%nvcc%" -arch=%shadermodel% -m64 --cubin %cyclesroot%/cycles/src/kernel/kernels/cuda/kernel.cu -o %cyclesout%/lib/kernel_%shadermodel%.cubin --ptxas-options="-v" -D__KERNEL_CUDA_VERSION__=%cudaversion% --use_fast_math -I%cyclesroot%/cycles/src/kernel/../util -I%cyclesroot%/cycles/src/kernel/svm -DCCL_NAMESPACE_BEGIN= -DCCL_NAMESPACE_END= -DNVCC
)
