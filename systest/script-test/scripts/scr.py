# sourceSet contains the source set to be built

import os
from os.path import basename

genExe = get_tool("fsrepo://MessageGenerator/*.*", "MessageGenerator.exe")

name = ''
for file in sourceSet:
	print "Script is processing " + file
	name = basename(file)

	with open(file, "r") as f:
		msg="Hello_" + f.read()

		cmd = genExe + ' "' + targetDir+os.path.sep+name+'.txt" "' + msg + '"'
                if is_mono:
                        cmd = 'mono ' + cmd
		print cmd
		os.system(cmd)

results = [name+".txt"]
