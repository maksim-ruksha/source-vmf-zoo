# Source VMF Zoo
A little tool for source engine maps. Original idea by @VanderAGSN.

# What it does?
* Produces .vmf file (zoo) with all props of inputted .vmf
* Finds corrupted props 
* Counts how many times each prop was used (in the produced .vmf they are placed in descending order)

# How to use it?
In both cases you must place template.txt next to executable file. This txt is used to specify basic map information. You can modify it for own needs. The one thing you should type "entities" in place you want to place entities.

### Way 1
Just drag your map's .vmf file to program's executable. Follow instructions on the screen. Your zoo file will be named "<input_file_name>_zoo.vmf".
### Way 2
Launch program's executable. In this case program will try to read input.vmf file which must be placed next to it. The result will be stored in output.vmf



