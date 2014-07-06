PMUServer
=========
The server EXE in the Build folder can be run as is.  However, the database must be set up.

You must:

Install MySQL, and run the database server.

When you create a user and password, enter the respective data in the config.xml file in the Build\Data folder.

To import the data used in PMU, unzip Content_Data.zip.

Edit the username and password data in the batch file, then run it.  Your MySQL database should now be populated with data.

run the server, and it should load properly