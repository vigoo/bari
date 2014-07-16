print 'Target dir:' + targetDir

with open(targetDir+'\\generated.txt', "w") as f:
	f.write('Hello world\n')

results = ['generated.txt']
