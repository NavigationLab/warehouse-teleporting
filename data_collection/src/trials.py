"""
File: trials.py
Descr: Represents a single trial run, e.g. a single triangle completion
Developed 2/13/2019 by Alec Ostrander
"""

from math import acos, degrees


class Trial:

    def __init__(self, num, green, yellow, red, answer, times):
        """
        :param num: int, the trial number
        :param green: tuple, the coordinates of the green marker
        :param yellow: tuple, the coordinates of the yellow marker
        :param red: tuple, the coordinates of the red marker
        :param answer: tuple, the coordinates of the participant's answer
        :param times: 4-tuple, the absolute timing of the trial, per below:
                 1: the trial start time
                 2: the time of teleport to green marker
                 3: the time of teleport to red marker
                 4: the time of the final participant answer
        """
        # store 'original' trial data
        self.trial_number = num
        self.markers_pos = {"green": green, "yellow": yellow, "red": red}
        self.answer_pos = answer
        self.times = times

        print("Trial:", num)

        # Compute rich trial data if the data is available

        # All the try-except clauses are for catching missing data.
        # In that case, a call for that attribute defaults to None via __getattr__()

        # The distance from the starting location to the green marker
        try: self.green_dist = self._distance((0,0), green)
        except TypeError: pass

        # The distance from the green marker to the yellow marker
        try: self.yellow_dist = self._distance(green, yellow)
        except TypeError: pass

        # The distance from the yellow marker to the red marker
        try:self.red_dist = self._distance(yellow, red)
        except TypeError: pass

        # The distance from the red marker back to the green marker
        try: self.completion_dist = self._distance(red, green)
        except TypeError: pass

        # The distance from the red marker to the point the user answered
        try: self.answer_dist = self._distance(red, answer)
        except TypeError: pass

        # Some vectors for calculating angles below
        v_startgreen = green
        try: v_greenyellow = tuple(yellow[i] - green[i] for i in range(len(green)))
        except TypeError: pass
        try: v_yellowred = tuple(red[i] - yellow[i] for i in range(len(green)))
        except TypeError: pass
        try: v_redgreen = tuple(green[i] - red[i] for i in range(len(green)))
        except TypeError: pass
        try: v_redanswer = tuple(answer[i] - red[i] for i in range(len(green)))
        except TypeError: pass

        # The number of degrees a user at the green marker turns to face the yellow marker
        try: self.green_angle = self._angle(v_startgreen, v_greenyellow)
        except UnboundLocalError: pass

        # The number of degrees a user at the yellow marker turns to face the red marker
        try: self.yellow_angle = self._angle(v_greenyellow, v_yellowred)
        except UnboundLocalError: pass

        # The number of degrees a user at the red marker turns to face where the green marker was
        try: self.tri_complete_angle = self._angle(v_yellowred, v_redgreen)
        except UnboundLocalError: pass

        # The number of degrees the user turned from the red marker to place their answer
        try: self.turned_angle = self._angle(v_yellowred, v_redanswer)
        except UnboundLocalError: pass

        # The X and Y distance the user's answer was from the green marker location
        try: self.cartesian_error = tuple(answer[i] - green[i] for i in range(len(green)))
        except TypeError: self.cartesian_error = (None, None)

        # The absolute distance between the user's answer and the green marker's location
        try: self.error_magnitude = self._distance(answer, green)
        except TypeError: pass

        # The error in how far from the red marker the user pointed to, and
        # and the error in how many degrees the user turned to point.
        try: self.polar_error = (self.answer_dist - self.completion_dist,
                                 self._angle(v_redgreen, v_redanswer))
        except TypeError: self.polar_error = (None, None)

        # The time from start to touching the green marker,
        # touching the red marker, and giving an answer
        if None in times:
            self.green_time = self.red_time = self.answer_time = None
        else:
            self.green_time = times[1] - times[0]
            self.red_time = times[2] - times[0]
            self.answer_time = times[3] - times[0]

    def __getattr__(self, name):
        """This fills in for any attributes that
        weren't initialized due to missing data"""
        return None

    @staticmethod
    def _distance(p1, p2):
        """Computes the Euclidean distance between two coordinates"""
        assert len(p1) == len(p2)
        return sum((p1[i] - p2[i]) ** 2 for i in range(len(p1))) ** 0.5

    @staticmethod
    def _angle(v1, v2):
        """Computes the angle in degrees from v1 to v2"""

        # Compute the angle of each vector from 0 (doing them separately
        # makes it easy to account for the direction of rotation)
        degs = []
        for vector in (v1, v2):
            print("\t\t", vector)
            deg = degrees(acos(vector[0] / sum(a ** 2 for a in vector) ** .5))
            if vector[1] < 0:
                deg *= -1
            degs.append(deg)

        # Compute the angle between vectors
        angle = degs[1] - degs[0]

        # Reduce the angle to the range -180  - 180
        while angle < -180:
            angle += 360
        while angle > 180:
            angle -= 360

        return angle
