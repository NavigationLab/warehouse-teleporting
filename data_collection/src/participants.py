"""
File: participants.py
Descr: Represents a single participant and contains all
       conditions in which the participant was run
Developed 2/13/2019 by Alec Ostrander
"""


from conditions import Condition


class Participant:

    def __init__(self, _id, gender):
        """
        :param _id: string, the participant's identifier
        :param gender: string, the participant's gender (M, F, or O)

        Participant objects hold the participant-wide data above,
        but are primarily a container for Condition objects.
        """

        # store participant information
        self._id = _id
        self.gender = gender
        self.conditions = []

        print("Participant:", _id)

    def add_condition(self, condition):
        self.conditions.append(condition)

    def pull_data(self, dirs, fs):
        """
        Parses the folders for each participant and
        generates the corresponding Condition objects.

        :param dirs: string, A list of directories relevant to this participant
        :param fs: string, may be provided to specify a different folder structure.
            (Environment is assumed to the be the 1st condition, separated by underscores)
        """

        # Find the folder structure levels for interface and conditions
        ui_level = fs.split("/").index("Interface")
        cond_level = fs.split("/").index("Conditions")

        # Initialize a temporary data structure for condition info
        conditions = {}

        # Loop through each directory for this participant and parse out the conditions
        for _dir in dirs:
            path = _dir[len("../Data")+1:]
            cond = (path.split("\\")[ui_level], path.split("\\")[cond_level])

            # If it's the first time seeing this condition, add it
            if cond not in conditions:
                conditions[cond] = []

            # Add the current directory to the list for this Condition
            conditions[cond].append(_dir)

        # Once we have all conditions for the participant, simply create
        # each object, populate it, and add it to the participant
        for c in sorted(conditions.keys()):
            ui = c[0]
            env = c[1].split("_")[0]
            conds = [] if "_" not in c[1] else c[1].split("_")[1]

            condition = Condition(ui, env, conds)
            condition.pull_data(conditions[c], fs)
            self.add_condition(condition)

    def __iter__(self):
        yield from self.conditions
