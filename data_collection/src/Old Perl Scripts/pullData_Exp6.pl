#! /usr/bin/perl -w

use strict;
use Math::Trig;

# This should be the directory with the data from Unity
my $dataDir = 'C://Users/Test/Desktop/TeleportData';
my @uiFolderList = glob($dataDir . '/*');
my @angleList = ( 135, 112.5, 90, 67.5, 45, 22.5, -22.5, -45, -67.5, -90, -112.5, -135 );
# Twelve trials are usually done, but if this changes, change this comment and $numTrials
my $numTrials = 12;

# This is the output file.
# It can be changed to whatever output file is needed
open(OUT, ">Data.csv");
print OUT "Participant,Interface,Condition,GreenX,GreenZ,YellowX,YellowY,RedX,RedZ,MarkerAngle,UserX,UserZ,ErrorX,ErrorY,ErrorMag,MarkerTime,TrialTime,AngleError\n";

foreach my $ui_folder (@uiFolderList){
    my @tmp = split(/\//, $ui_folder);
	my $tmp1 = $tmp[-1];
	if ($tmp1 eq "Discordant" || $tmp1 eq "Hybrid") {
	    my @condFolderList = glob($ui_folder . '/*');
		foreach my $cond_folder (@condFolderList) {
		    @tmp = split(/\//, $cond_folder);
			my $tmp2 = $tmp[-1];
		    if ($tmp2 eq "Training" || $tmp2 eq "WarehouseBig_1x" || $tmp2 eq "WarehouseBig_4x" || $tmp2 eq "WarehouseSmall_1x" || $tmp2 eq "WarehouseSmall_4x") {
                my @participantList = glob($cond_folder . "/*");
                foreach my $p_folder (@participantList) {
                    if ($p_folder =~ m/.+?(\d+)\_([MFO])/){
					    writeData($p_folder, $tmp2, $tmp1, $1);
					} else {
					    die "$p_folder is not in the correct format!";
					}
				}
			} else {die "Error 2: $cond_folder";}
		}
	} else {die "Error 1: $ui_folder";}
}


# This parses the files and writes the results to the output file
sub writeData {
	my ($dir, $condition, $interface, $participant) = @_;
	# print "$dir\n";


	my ($marker, $place, $response, $time, $track)  = glob("$dir/*");
	my $markerData = read_file($marker);
	my @startMarkers = $markerData =~ /Starting Marker.+?\nFirst Marker.+?\nSecond Marker.+?\n-+New Trial-+/g;
	
	my $responseData = read_file($response);
	my @responseList = $responseData =~ /Response:.*?\n-+New Trial-+/g;

	my $timeData = read_file($time);
	my @startTimes = $timeData =~ /Start marker time: -?\d+\.?\d*\nSecond marker time: -?\d+\.?\d*\n-+New Trial-+/g;
	
	my @teleportList = findTeleports($place);

	# print "@teleportList\n";
	if($#responseList != $#startTimes){
		print "$dir has an error with responses and start times!\n";
	}
	if($#responseList != $#startMarkers){
		print "$dir has an error with responses and start markers!\n";
	}
	for(my $i = 0; $i <= $#responseList; $i++){
	# if($teleportList[$i] > 3){
	# #	next;
	# }
	if($startMarkers[$i] =~ m/Starting.+:\((-?\d+\.?\d*), .+?, (-?\d+\.?\d*)\)\nFirst Marker:\((-?\d+\.?\d*), .+?, (-?\d+\.?\d*)\)\nSecond.+?\((-?\d+\.?\d*), .+?, (-?\d+\.?\d*)\)/)
	{
			#print "$startMarkers[$i]\n\n";
			my ($startX, $startY, $firstX, $firstY, $secondX, $secondZ) = ($1, $2, $3, $4, $5, $6);
			my $markerAngle = calculateMarkerAngle($firstX, $firstY, $startX, $startY, $secondX, $secondZ);
			if($startTimes[$i] =~ m/Start marker time: (-?\d+\.?\d*)\nSecond marker time: (-?\d+\.?\d*)\n-+New Trial-+/){
				my ($trialStart, $markerHit) = ($1, $2);
				if($responseList[$i] =~ m/Response.+\((-?\d+\.?\d*), .+?, (-?\d+\.?\d*)\) Time: (\d+\.?\d*)\n-+New Trial-+/) {
					my $angle = calculateAngle($secondX, $secondZ, $startX, $startY, $1, $2);
					my $errorX = $1 - $startX;
					my $errorY = $2 - $startY;
					my $errorMag = sqrt($errorX * $errorX + $errorY * $errorY);
					if($3 - $markerHit < 0) {
						print "$3 $markerHit $dir\n";
					}
					print OUT "$participant,$interface,$condition,$startX,$startY,$firstX,$firstY,$secondX,$secondZ,$markerAngle,$1,$2,$errorX,$errorY,$errorMag," . ($3 - $markerHit) . "," .($3 - $trialStart) . ",$angle\n";
				}
			}
			else {
				my ($trialStart, $markerHit) = ($1, $2);
				if($responseList[$i] =~ m/Response.+\((-?\d+\.?\d*), .+?, (-?\d+\.?\d*)\) Time: (\d+\.?\d*)\n-+New Trial-+/) {
					my $angle = calculateAngle($secondX, $secondZ, $startX, $startY, $1, $2);
					my $errorX = $1 - $startX;
					my $errorY = $2 - $startY;
					my $errorMag = sqrt($errorX * $errorX + $errorY * $errorY);
					if($3 - $markerHit < 0) {
						print "$3 $markerHit $dir\n";
					}
					print OUT "$participant,$interface,$condition,$startX,$startY,$firstX,$firstY,$secondX,$secondZ,$markerAngle,$1,$2,$errorX,$errorY,$errorMag, , ,$angle\n";
				}
			}
		}
	}
}

# This calculates the angle between two lines
sub calculateAngle{
	my ($originX, $originY, $x1, $y1, $x2, $y2) = @_;
	my @v1 = ($x1 - $originX, $y1 - $originY);
	my @v2 = ($x2 - $originX, $y2 - $originY);
	my $mag = ($v2[1] * $v1[1] + $v2[0] * $v1[0]) / (sqrt($v2[1] * $v2[1] + $v2[0] * $v2[0]) * sqrt($v1[1] * $v1[1] + $v1[0] * $v1[0]));
	my $angle = acos($mag);
	my $cross = ($v1[0] * $v2[1] - $v1[1] * $v2[0]);
	if ($cross > 0){
		$angle = $angle * -1;
	}
	# print "@_ | @v1 | @v2 | $mag | $angle\r\n";
	return rad2deg($angle);
}

# The marker angle isn't given and using arctan gives rouding errors
# Thus, this will give the exact angle by finding the closest angle in @angleList
sub calculateMarkerAngle{
	my ($originX, $originY, $x1, $y1, $x2, $y2) = @_;
	my @v1 = ($x1 - $originX, $y1 - $originY);
	my @v2 = ($originX - $x2, $originY - $y2);
	my $mag = ($v2[1] * $v1[1] + $v2[0] * $v1[0]) / (sqrt($v2[1] * $v2[1] + $v2[0] * $v2[0]) * sqrt($v1[1] * $v1[1] + $v1[0] * $v1[0]));
	my $angle = acos($mag);
	my $cross = ($v1[0] * $v2[1] - $v1[1] * $v2[0]);
	if ($cross > 0){
		$angle = $angle * -1;
	}
	# print "@_ | @v1 | @v2 | $mag | $angle\r\n";
	my $radAngle = rad2deg($angle);

	my $smallAngle = 9001;
	my $champAngle = 9001;
	foreach my $angle (@angleList)
	{
		my $diff = abs($radAngle - $angle);
		#print "abs($radAngle - $angle) < $smallAngle $champAngle\n";

		if($diff < $smallAngle)
		{
			$champAngle = $angle;
			$smallAngle = $diff;
		}
	}
	 # print ($radAngle - $champAngle);
	 # print ",$radAngle,$champAngle\r\n";
	 return $champAngle;
	}

# Returns absolute values
sub abs {
	my ($num) = @_;
	if ($num < 0){
		$num = $num * -1;
	}
	return $num;
}

# Slurp read_file() because not all computers have slurp installed
sub read_file() {
	my ($file) = @_;
	my $result = "";
	open (IN, "<$file");
	while(<IN>){
		$result = $result . $_;
	}
	close(IN);
	return $result;
}

# This returns a list of number of teleportations that were done in a test.
sub findTeleports {
	my ($file) = @_;
	print "$file\n";
	open(IN, '<' . "$file") or die("NOPE");
	my $line = <IN>;
	my $compressedFile = "";
	while (<IN>){
		my $temp = $_;
		if( $temp =~ m/-+New Trial-+/){
			$line = "";
			$compressedFile = $compressedFile . "$temp";
			next;
		}
		if( $line =~ m/.+Camera rig position(.+) Eyes .+/){
			my $pos = $1;
			if($temp =~ m/.+Camera rig position(.+) Eyes .+/){
				if($pos ne $1)
				{
					$compressedFile = $compressedFile . "$pos $1\n";
				}
			}
		}
		$line = $temp;
	}
	my @trialList = split(/\n-+New Trial-+/, $compressedFile);
	while (scalar @trialList > $numTrials){
		pop @trialList;
	}
	my @result = ();
	foreach my $trial (@trialList){
		chomp $trial;
		if($trial =~ s/^\s+//){}
		my @positionList = split("\n", $trial);
		push (@result, scalar @positionList);
	}
	return @result;
}
