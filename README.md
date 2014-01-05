bari
====

Bari is an advanced build management tool for .NET projects.

The project is already usable for smaller projects, but there is no documentation available yet.

# Roadmap #

## Milestone 1 ##

* Bari will be able to compile and test itself.

## Milestone 2 ##

* Bari is able to compile one of my more complex projects with multiple _modules_ (currently as Visual Studio solutions) and custom build steps.

## Milestone 3 (release v1) ##

* Bari is tested and used by the team developing the project mentioned in Milestone 2
* Bari will be fully documented with examples and howto-s on its website


# Getting started #
## Getting bari ##

Bari itself is now compiled using bari, so you'll have to download the latest version as a binary package. 

The [latest version is 0.7](https://github.com/vigoo/bari/releases/tag/0.7).

## Documentation ##
Documentations is not available yet. Until it's ready, I recommend browsing the following suite definitions for examples:

* [Simple C# executable](https://github.com/vigoo/bari/blob/master/systest/single-cs-exe/suite.yaml)
* [Simple F# executable](https://github.com/vigoo/bari/blob/master/systest/single-fs-exe/suite.yaml)
* [Simple C++ executable](https://github.com/vigoo/bari/tree/master/systest/single-cpp-exe)
* [Dependencies within a module](https://github.com/vigoo/bari/blob/master/systest/module-ref-test/suite.yaml)
* [Dependencies within a suite](https://github.com/vigoo/bari/blob/master/systest/suite-ref-test/suite.yaml)
* [Content files support](https://github.com/vigoo/bari/blob/master/systest/content-test/suite.yaml)
* [Support for reference aliases](https://github.com/vigoo/bari/blob/master/systest/alias-test/suite.yaml)
* [File system repository](https://github.com/vigoo/bari/blob/master/systest/fsrepo-test/suite.yaml) 
* [C++ static library support](https://github.com/vigoo/bari/blob/master/systest/static-lib-test/suite.yaml)
* [C++ resource support](https://github.com/vigoo/bari/blob/master/systest/cpp-rc-support/suite.yaml)
* [C++/CLI support](https://github.com/vigoo/bari/blob/master/systest/mixed-cpp-cli/suite.yaml)
* [Registration-free COM support](https://github.com/vigoo/bari/blob/master/systest/regfree-com-server/suite.yaml)

And the [Bari suite definition](https://github.com/vigoo/bari/blob/master/suite.yaml) itself! 

To get a list of available commands, use
`bari help`.