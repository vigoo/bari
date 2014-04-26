# sourceSet contains the source set to be built

import os
from os.path import basename

genExe = get_tool("fsrepo://MessageGenerator/*.*", "MessageGenerator.exe")

name = ''
for file in sourceSet:
	print "Script is processing " + file
	name = basename(file)

	with open(file, "r") as f:
		msg="Hello " + f.read()		
		cmd = genExe + ' "' + targetDir+'\\'+name+'.txt" "' + msg + '"'
		print cmd
		os.system(cmd)

results = [name+".txt"]