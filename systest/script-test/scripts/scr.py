# sourceSet contains the source set to be built

for file in sourceSet:
	print "Script is processing " + file

	with open(file, "r") as f:
		msg="Hello " + f.read()
		with open(targetDir+"/message.txt", "w") as out:
			out.write(msg)

results = ["message.txt"]