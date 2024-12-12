File that we write in the changes in the Project
# dotNet5785_3252_1013
Windows project. Creating a volunteer management system
/stage0 final commit/

/stage1 final commit/
$ We add Trypharse bonus -  1 point$
	We couldn't define the variables inside the loop because we used them later,
	but we did capture the variable if it was entered incorrectly again inside the loop.

$we add password bonus-2 points$

/stage2 final commit/

/stage3 final commit/

$ We add Thread Safe full Lazy Initialization bonus- 2 points$

in volunteerImplemenation of BO we decided to let the system delete a volunteer just if he do not have a call in program, 
becouse otherwise even if he never treated a call but now he has a call in progress when we delete the volunteer it will create
problemswith the current call

in volunteerManager of BO we wrote a functions for each field that need check,that include every check he need include numbers etc.
and that why we didnt write a func that check if number field is a real number, and about id we assume that as an int he will
throw exeption earlier if he got non int value, and if we got a field of int he must be valid for int

in the func create of volunteerImplemenation we assume that the person who created the volunteer is manager becouse
we doesent have any information about the person who add the volunteer and only manager can add

$we added bonus of check that the password is strong-1 point $

$we added bonus of encode and decipher the password- 2 points$






