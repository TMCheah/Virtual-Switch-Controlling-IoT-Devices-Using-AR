# Virtual-Switch-Controlling-IoT-Devices-Using-AR
Final Year Project (BSc (Hons) Computer Science)

### Description
Controlling electronic switches (such as light) virtually from smartphone with interactive AR switches.
In this project, a 2-story house was modeled to be used as a prove of concept for this project. There are total of 7 rooms with 7 light hook up to IoT Controller, ESP8266 as a web server. User able to select specific floor of the house to select the specific room to toggle the virtual switch to trigger the actual electronic.
This project is also design to be as expandable as possible, meaning the number of devices connected to the internet is able to expand.

#### Features
- Local Area Network Setup
- Web Server Admin Controller page
- Expandable IoT device to be connected
- AI Recommendation System
- Old Smartphone was supported (Android 7 and above)

### Project Structure
This project consist of 2 parts
- Mobile Phone, develop using Unity with Vuforia Engine
- Web Server, develop using ESP8266 on Arduino IDE

![Project Illustration](https://github.com/TMCheah/Virtual-Switch-Controlling-IoT-Devices-Using-AR/blob/main/Hardware%20setup.png)

#### Modeling tools
House was model in Minecraft. It is then converted to Obj format using mc2obj, link to the [GitHub page](https://github.com/jmc2obj/j-mc-2-obj) here.

#### AR Engine
Vuforia Engine was use in this project.
3D Marker used is a QR code.

### Some screenshot preview of this project
![3D view of the house use](https://github.com/TMCheah/Virtual-Switch-Controlling-IoT-Devices-Using-AR/blob/main/ProjectScreenshot/obj/House1.png)
![Web server landing page](https://github.com/TMCheah/Virtual-Switch-Controlling-IoT-Devices-Using-AR/blob/main/Screenshot/landing%20page.png)
