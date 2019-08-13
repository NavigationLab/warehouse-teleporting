"""
File: conditions.py
Descr: Represents a single condition and contains
       all trials run with that condition
Developed 2/13/2019 by Alec Ostrander
"""


from reader import ExpReader
from trials import Trial


class Condition:

    def __init__(self, interface, environment, conditions):
        """
        :param interface: string, the navigation interface being used in this condition
        :param environment: string, the name of the virtual environment being used
        :param conditions: list, other config variables unique to the environment
                 e.g fence presence and shape in the field

        Condition objects hold the condition-wide data above,
        but are primarily a container for Trial objects.
        """

        # store condition information
        self.interface = interface
        self.environment = environment
        self.conditions = conditions
        self.trials = []

        print("\tCondition:", interface, environment, conditions)

    def add_trial(self, trial):
        self.trials.append(trial)

    def pull_data(self, dirs, fs):
        """
        Parses the folders for each condition and
        generates the corresponding Trial objects.

        :param dirs: string, a list of directories relevant to this condition
        :param fs: string, may be provided to specify a different folder structure.
        """

        # Loop through each directory for this participant-condition
        for _dir in dirs:

            # Parse trial parameters from the raw data files
            reader = ExpReader(_dir)
            markers = reader.get_marker_locations()
            answers = reader.get_answers()
            timings = reader.get_timings()

            # Make sure everything parsed alright
            if not (len(markers) == len(answers) == len(timings)):
                print("Warning: Unknown error parsing files for {}.".format(_dir))
                print((len(markers), len(answers), len(timings)))

            # Create each Trial object and add it to the condition
            for i in range(len(markers)):
                timings[i].append(answers[i][1])
                trial = Trial(i+1, markers[i][0], markers[i][1], markers[i][2], answers[i][0], timings[i])
                self.add_trial(trial)

    def __iter__(self):
        yield from self.trials
