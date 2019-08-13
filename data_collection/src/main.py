"""
File: main.py
Descr: Top-level file for processing data with this package.
       Use this to do actual data processing; the other files only
       need to be touched in the case of major structural changes
Developed 2/13/2019 by Alec Ostrander
"""


import csv
import datetime
from experiments import Experiment


# ________________________________ Write To CSV _____________________________________________

# Use this to generate a new CSV file with rich trial data
def write_to_csv(exp, fname):

    # Open a new csv for the experiment in the output folder
    with open(fname, "w", newline="") as csvfile:
        writer = csv.writer(csvfile)

        # Add the column headers
        head = ["Participant", "Gender", "Interface", "Environment", "Condition", "Trial",
                "GreenX", "GreenY", "GreenDist", "GreenAngle",
                "YellowX", "YellowY", "YellowDist", "YellowAngle",
                "RedX", "RedY", "RedDist",
                "CompletionDist", "TriangleCompletionAngle",
                "AnswerX", "AnswerY", "AnswerDist", "AnswerTurnAngle",
                "ErrorX", "ErrorY", "ErrorMag", "AxialError", "AngularError",
                "TimeToGreen", "TimeToRed", "TimeToAnswer"]
        writer.writerow(head)

        # Loop through each participant...
        for part in exp:
            # through each of the participant's conditions...
            for cond in part:
                # through each trial they did in that condition
                for trial in cond:

                    # Create the data row
                    # Participant ID, gender, interface, environment, conditions, and trial #
                    row = [part._id, part.gender, cond.interface,
                           cond.environment, cond.conditions, trial.trial_number]
                    row.extend(trial.markers_pos["green"])   # Green marker position (X, Y)
                    row.append(trial.green_dist)             # Distance from start to green marker
                    row.append(trial.green_angle)            # Turn angle from start-green-yellow
                    row.extend(trial.markers_pos["yellow"])  # Yellow marker position (X, Y)
                    row.append(trial.yellow_dist)            # Distance from green to yellow

                    try:  # Round the angle to the nearest 'step' (e.g. 22.5 for 12 trials)
                        angle_step = 270 / len(cond.trials)
                        trial.yellow_angle = angle_step * round(trial.yellow_angle / angle_step)
                    except TypeError: pass

                    row.append(trial.yellow_angle)           # Turn angle from green-yellow-red
                    row.extend(trial.markers_pos["red"])     # Red marker position (X, Y)
                    row.append(trial.red_dist)               # Distance from yellow to red
                    row.append(trial.completion_dist)        # Distance from red back to green
                    row.append(trial.tri_complete_angle)     # Turn angle from yellow-red-green
                    row.extend(trial.answer_pos)             # Answer position (X, Y)
                    row.append(trial.answer_dist)            # Distance from red to answer
                    row.append(trial.turned_angle)           # Turn angle from yellow-red-answer
                    row.extend(trial.cartesian_error)        # Absolute positional error (X, Y)
                    row.append(trial.error_magnitude)        # Distance from answer to green
                    row.extend(trial.polar_error)            # Distance and angular error
                    row.append(trial.green_time)             # Time from start to green marker
                    row.append(trial.red_time)               # Time from start to red marker
                    row.append(trial.answer_time)            # Time from start to answer

                    # Write the row to csvfile
                    writer.writerow(row)


# ________________________________ Perl Format CSV _____________________________________________

# Use this to generate the exact same CSV as from the old Perl scripts
def perl_formatted_csv(exp, fname):

    # Open a new csv for the experiment in the output folder
    with open(fname, "w", newline="") as csvfile:
        writer = csv.writer(csvfile)

        # Add the column headers
        head = ["Participant", "Interface", "Condition",
                "GreenX", "GreenZ", "YellowX", "YellowY", "RedX", "RedZ", "MarkerAngle",
                "UserX", "UserZ", "ErrorX", "ErrorY", "ErrorMag",
                "MarkerTime", "TrialTime", "AngleError"]
        writer.writerow(head)

        # Loop through each participant...
        for part in exp:
            # through each of the participant's conditions...
            for cond in part:
                # through each trial they did in that condition
                for trial in cond:

                    # Create the data row
                    # Participant ID, the interface, the environment and any conditions
                    row = [part._id, cond.interface, cond.environment+cond.conditions]
                    row.extend(trial.markers_pos["green"])     # GreenX and GreenZ
                    row.extend(trial.markers_pos["yellow"])    # YellowX and YellowY
                    row.extend(trial.markers_pos["red"])       # RedX and RedZ

                    # Calculate MarkerAngle as long as the positional data is there
                    try: row.append(-trial.yellow_angle)
                    except TypeError: row.append("")
                    row.extend(trial.answer_pos)               # UserX and UserZ
                    row.extend(trial.cartesian_error)          # ErrorX and ErrorY
                    row.append(trial.error_magnitude)          # ErrorMag

                    # Calculate MarkerTime as long as the time data is there
                    try: row.append(trial.answer_time - trial.red_time)
                    except TypeError: row.append("")

                    # Calculate TrialTime as long as the time data is there
                    try: row.append(trial.answer_time - trial.green_time)
                    except TypeError: row.append("")

                    # Calculate AngleError as long as the positional data is there
                    try: row.append(-trial.polar_error[1])
                    except TypeError: row.append("")

                    # Write the row to csvfile
                    writer.writerow(row)


# _________________________________ Main ________________________________________________

if __name__ == "__main__":

    # Initialize the experiment with a title from console
    title = input("Enter a name for the experiment: ")
    experiment = Experiment(title)

    # Map data into the experiment objects
    print("Pulling data...", end="")
    experiment.pull_data()
    print("Done.")

    # Write the data to <Title>_data_<Today>.csv
    file_name = "Output/{}_data_{}.csv".format(
                 experiment.name, datetime.datetime.today().date())
    print("Writing to {}...".format(file_name), end="")

    # ----- SELECT YOUR OUTPUT TYPE HERE ------------------------------------------------

    # Uncomment this function to generate the rich data CSV made by Alec
    write_to_csv(experiment, "../" + file_name)

    # Uncomment this function to generate the same CSV as the old Perl script did
    # perl_formatted_csv(experiment, file_name)

    # ----- SELECT YOUR OUTPUT TYPE HERE ------------------------------------------------

    print("Done.")
    input("Press any key to exit...")
