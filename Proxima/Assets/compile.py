import subprocess
import glob
import os 
files = [os.path.splitext(file)[0] for file in glob.glob("*.vert")]

for file in files:
	subprocess.run(["glslc", f"{file}.vert", "-o", "temp.vert.spv"])
	subprocess.run(["glslc", f"{file}.frag", "-o", "temp.frag.spv"])
	
	subprocess.run(["spirv-link", "temp.vert.spv", "temp.frag.spv", "-o", file+".spv"])

	subprocess.run(["rm", "temp.vert.spv"])
	subprocess.run(["rm", "temp.frag.spv"])