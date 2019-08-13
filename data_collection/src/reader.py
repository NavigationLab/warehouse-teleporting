"""
File: reader.py
Descr: Specialized class for reading the 5 raw data files
Developed 2/22/2019 by Alec Ostrander
"""


import re


class ExpReader:

    def __init__(self, _dir):
        """
        :param _dir: string, the directory containing the experiment raw data files
        """

        self._dir = _dir

    def get_marker_locations(self):

        # Initialize storage for the marker locations
        markers = []

        # The loop starts with this list of tuples and populates it with the marker
        # locations before adding it to markers. Doing it this way makes sure
        # something is always added for the trial, even if the data file is faulty
        trial_set = [(None, None), (None, None), (None, None)]

        # Loop through the Marker.txt file and parse marker coordinates for each trial
        with open(self._dir + "/Marker.txt") as mfile:
            for line in mfile:

                if "Restart" in line:
                    # If it's a "Restart" line, erase the set and start the trial over
                    trial_set = [(None, None), (None, None), (None, None)]

                elif "New Trial" in line:
                    # If it's a "New Trial" line, log the set and prep the next trial
                    markers.append(trial_set)
                    trial_set = [(None, None), (None, None), (None, None)]

                elif line != "":
                    # As long as it's not the end of the file, then this line has marker
                    # data for the trial. Parse it out and save in the trial_set
                    marker, x, _, z, _ = re.split(":\(|, |\)", line.strip())
                    coords = (float(x), float(z))
                    if marker == "Starting Marker":
                        trial_set[0] = coords
                    elif marker == "First Marker":
                        trial_set[1] = coords
                    elif marker == "Second Marker":
                        trial_set[2] = coords
                    else:
                        print("Error Reading {}/Marker.txt".format(self._dir))

        return markers

    def get_answers(self):

        # Initialize storage for the answer locations and times
        answers = []

        # The loop starts with this list and populates it with the answer location
        # and time before adding it to answers. Doing it this way makes sure
        # something is always added for the trial, even if the data file is faulty
        trial_set = [(None, None), None]

        # Loop through Response.txt and parse answer coordinates and times for each trial
        with open(self._dir + "/Response.txt") as rfile:
            for line in rfile:

                if "Restart" in line:
                    # If it's a "Restart" line, erase the set and start the trial over
                    trial_set = [(None, None), None]

                elif "New Trial" in line:
                    # If it's a "New Trial" line, log the set and prep the next trial
                    answers.append(trial_set)
                    trial_set = [(None, None), None]

                elif line != "":
                    # As long as it's not the end of the file, then this line has
                    # answer data for the trial. Parse it out and save in the trial_set
                    _, _, x, _, z, _, time = re.split("\(|, |\)|: ", line.strip())
                    trial_set[0] = (float(x), float(z))
                    trial_set[1] = float(time)

        return answers

    def get_timings(self):

        # Initialize storage for times
        times = []

        # The loop starts with this initial list and populates it with the trial
        # times before adding it to times. Doing it this way makes sure something
        # is always added for the trial, even if the data file is faulty
        trial_set = [None, None, None]

        # Loop through the Time.txt file and parse times for each event
        with open(self._dir + "/Time.txt") as tfile:
            for line in tfile:

                if "Restart" in line:
                    # If it's a "Restart" line, erase the set and start the trial over
                    trial_set = [None, None, None]

                elif "New Trial" in line:
                    # If it's a "New Trial" line, log the set and prep the next trial
                    times.append(trial_set)
                    trial_set = [None, None, None]

                elif line != "":
                    # As long as it's not the end of the file, then this line has
                    # timing data for the trial. Parse it out and save in the trial_set
                    marker, time = line.strip().split(":")
                    if marker == "Start time":
                        trial_set[0] = float(time)
                    elif marker == "Start marker time":
                        trial_set[1] = float(time)
                    elif marker == "Second marker time":
                        trial_set[2] = float(time)
                    else:
                        print("Error Reading {}/Time.txt".format(self._dir))

        return times
