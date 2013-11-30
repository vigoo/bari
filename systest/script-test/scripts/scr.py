# sourceSet contains the source set to be built

import os

genExe = get_tool("fsrepo://MessageGenerator/*.*", "MessageGenerator.exe")

for file in sourceSet:
	print "Script is processing " + file

	with open(file, "r") as f:
		msg="Hello " + f.read()
		cmd = genExe + ' "' + targetDir+'\\message.txt" "' + msg + '"'
		print cmd
		os.system(cmd)

results = ["message.txt"]