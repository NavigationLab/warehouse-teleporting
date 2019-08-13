# warehouse-teleporting

This project is part of the second in a series of studies conducted at the Iowa State University [Navigation Lab](https://navlab.psych.iastate.edu/) focusing on spatial cognition in virtual reality. For a general overview of the project and details on the first study, see [virtual-teleporting](https://github.com/NavigationLab/virtual-teleporting).

Experiment demonstration videos, data, and other materials related to these experiments will be made available on [osf.io](https://osf.io/) over the coming months.

### The Experimental Task 
Following the same protocols as in [virtual-teleporting](https://github.com/NavigationLab/virtual-teleporting), experimenters led participants through the [triangle completion task](https://books.google.com/books?id=zN_WAgAAQBAJ&pg=PA86) using both the "Partially Concordant" and "Discordant" teleporting interfaces. Two additions were made:
1. Triangle path lengths were varied between "1x" and "4x" conditions.
2. The area of the environment was varied between 1406m^2 and 5625m^2.

### Usage
This project may be used to replicate the results found in our presented work. Two versions of the application are provided:
1. To run the application as was done in our experiments, a compiled version is provided. Simply download the project and run the "WarehouseTeleport.exe" executable file inside the build folder.

2. To view source code or make changes to the project, you will need to open the project in Unity. The project was developed in and is known to work with Unity v2018.2.14f1, though it should be compatible with any subsequent versions at the time of this writing with only minor modifications. The experiment workflow including triangle poles, teleport interfaces, and a sample environment are present, but certain assets could not be legally included due to licensing constraints. To run the full experiment application from source, you will need to buy and integrate the [Big Warehouse Pack](https://assetstore.unity.com/packages/3d/environments/industrial/big-warehouse-pack-96082) from the Unity Asset Store.

Upon running the application, experiment data will be stored in ```ViveGame-master/Data/```. To process the data for analysis, copy the ```Data``` folder into ```data_collection``` and use Python 3 to run the script ```src/main.py```. Python will generate an adjacent ```Output``` folder containing the processed data. See the README file within data_collection for more details.

### Contact
This project was developed by the Iowa State University Navigation Lab located in the Department of Psychology and affiliated with the Human-Computer Interaction Program. For questions or comments regarding this project, please contact principal investigator [Dr. Jonathan Kelly](mailto:jonkelly@iastate.edu).