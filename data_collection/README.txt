Project: Teleport Project Data Management
Author: Alec Ostrander
Date: 03/13/2019

Description: 
	Use this project to process raw data files from the Teleport project experiments. The
	program scans the data folder structure to identify all participants run in the experiment
	and the conditions they were run through, then parses the data files to extract trial data.
	The  object structure stores this data in an intuitive way, and the main.py file demonstrates
	how to use it to output data in a couple different ways.

How to Use:
	Place your data in the "Data" folder. This is where the program will look to grab and process
	the files. They should still be in the original folder structure output from Unity.

	Double-click the "Process Data" link to run the program. The program will ask for an experiment
	name to name the output file with, and then you should see the script processing the data and 
	writing to CSV. If the script runs successfully, your output CSV file will be in the "Output" 
	folder, labeled with the experiment name you gave.

Content Overview:
	This program follows a hierarchical structure defined by the objects described below. For
	further documentation, see the code commentation.

		- The Experiment object holds experiment-level data (name) and contains a
		  Participant object for every participant run in that experiment.

		- The Participant object holds participant-level data (ID and gender) and contains
		  a Condition object for every condition that participant was subjected to.

		- The Condition object holds condition-level data (the navigation interface, the
		  Unity environment, and any other environment-specific conditions) and contains
		  a Trial object for every trial run for that participant under those conditions.

		- The Trial object holds the majority of the data being processed, including the
		  trial number, marker locations, user actions, error metric values and trial
		  timings.

	The extra file, setup.py, is used to build the executable application. to build a new version,
        open a command prompt and enter "python setup.py build". Google "cx_freeze" for details.