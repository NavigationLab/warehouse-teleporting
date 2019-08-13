"""
File: experiments.py
Descr: Represents a full study experiment and contains
       all the participants run in that experiment
Developed 2/13/2019 by Alec Ostrander
"""

import os
from participants import Participant


class Experiment:

    def __init__(self, name):
        """
        :param name: string, a name given to the experiment.
                 e.g. "Individual Differences Study"

        Experiment objects hold any experiment-wide data above,
        but are primarily a container for Participant objects.
        """

        # store experiment information
        self.name = name
        self.participants = []

    def add_participant(self, participant):
        self.participants.append(participant)

    def pull_data(self, fs="Interface/Conditions/Participant"):
        """
        Parses the folders in the Data folder and populates the
        experiment with newly generated Participant objects.

        :param fs: string, may be provided to specify a different folder structure.
            (Environment is assumed to the be the 1st condition, separated by underscores)
        """

        # If fs was changed, make sure it still has all the necessary parts
        for level in ("Interface", "Conditions", "Participant"):
            assert level in fs

        # Find the folder structure level for participants
        level = fs.split("/").index("Participant")

        # Initialize a temporary data structure for participant info
        participants = {}

        # Recursively loop through the data directory to find all participants
        for path, dirs, files in os.walk("../Data"):

            if not dirs:          # Once at the bottom level,

                # grab the participant info from folder names
                p_id = path[len("../Data")+1:].split("\\")[level]

                # If it's the first time seeing this participant, add them
                if p_id not in participants:
                    participants[p_id] = []

                # Add the current directory to the list for this participant
                participants[p_id].append(path)

        # Once we have all participants in the experiment, simply create
        # each object, populate it, and add it to the experiment
        for p in sorted(participants.keys()):
            _id, gender = p.split("_")
            if _id[0] == "P":
                _id = _id[1:]
            part = Participant(_id, gender)
            part.pull_data(participants[p], fs)
            self.add_participant(part)

    def __iter__(self):
        yield from self.participants
