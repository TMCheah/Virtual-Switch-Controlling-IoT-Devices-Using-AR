/*EEPROM ADDRESS
 * 0 - 9  -> IoT device's state
 * 10-14  -> authorize device (able to scale)
 * 
 * 30-35  -> login password
 * 36     -> check if password is default or changed
 * 37     -> check if logged in?
 * 38	  -> check if IoT device were added initially
 * 39	  -> IoT device count
 * 40	  -> room Count
 * 50-59  -> room allocation of device
 * 60-69  -> IoT device type
 * 
 */

#include <ESP8266WiFi.h>
#include <ESP8266WebServer.h>
#include <EEPROM.h>
const char* ssid = "CHANGE TO YOUR SSID";
const char* password = "CHANGE TO YOUR PASSWORD";
const int passwordLength = 6;
const int maxIOTDevice = 9;			//LIMIT number of IOT device to connect
const int maxControlledDevice = 5; 	//LIMIT number of device with access
int countControlledDevice = 0;	//number of device with access.

//EEPROM ADDRESSES
const int EEPROM_password_address = 30;
const int EEPROM_password_change_status = 36;
const int IoT_device_initialize_status = 38;
const int IoT_device_count = 39;
const int room_count = 40;
const int room_allocation = 50;
const int device_type = 60;

int IoT_EEPROM_address[maxIOTDevice];
uint8_t IoT_IO[maxIOTDevice] = {D0,D1,D2,D3,D4,D5,D6,D7,D8};	 //D4 set as output will turn on the blue LED (internal)...
bool IoT_state[maxIOTDevice];	//RETHINK THE FUNCTION
bool IoT_Flag_state[maxIOTDevice];	//true = using , false = free, should be false at all time, true indicate using then will turn false after execution
int IoT_room_allocation[maxIOTDevice]; //track device in room
int IoT_device_type[maxIOTDevice]; //track device type

//CD = Controlled DEVICE
int EEPROM_CD[maxControlledDevice];
//const int EEPROM_CD1 = 10;
//const int EEPROM_CD2 = 11;
//const int EEPROM_CD3 = 12;
//const int EEPROM_CD4 = 13;
//const int EEPROM_CD5 = 14;

//global string variable for HTML
//String pageIndex;// = "";
//String pageLogin;// = "";
//String pagePW;// = "";
//String pageAD;//AD = add device
//String pageVD;//DRA = device-room allocation
String rootIP = "http://192.168.1.200/";
//String rootIP = "http://192.168.43.104/";//hotspot

//general Variable
int deviceNum;

//WiFiServer client(80);
ESP8266WebServer server(80);
//configure the IP
IPAddress ip(192,168,1,200);   
IPAddress gateway(192,168,1,254);   
IPAddress subnet(255,255,255,0);   

//====INIT====
void init_EEPROM_CD()
{
  for(int i = 0; i < maxControlledDevice; i++)
  {
    EEPROM_CD[i] = 10+i;
    if(EEPROM.read(EEPROM_CD[i])!=255)
      countControlledDevice++;  
  }  
}

void init_default_password()
{
  if(EEPROM.read(EEPROM_password_change_status) == 255)
  {
    for(int i = 0; i < passwordLength; i++)
    {
        EEPROM.write(EEPROM_password_address+i, (i+1));
    }
    EEPROM.commit();
  }
}

void init_IoT_EEPROM_address()
{
	for(int i = 0; i < maxIOTDevice; i++)
	{
		IoT_EEPROM_address[i] = i;	//EEPROM ADDRESS FROM 0 TO 9
		IoT_state[i] = EEPROM.read(IoT_EEPROM_address[i]);		//independent function returning specific address?
		IoT_Flag_state[i] = false;
	}
}

void init_pinMode()
{
	for(int i = 0; i < maxIOTDevice; i++)
	{
		pinMode(IoT_IO[i], OUTPUT);
		if(i<EEPROM.read(IoT_device_count))
		  digitalWrite(IoT_IO[i], IoT_state[i]);
	}
}

void init_deviceRoom_nType()
{
	for(int i = 0; i < maxIOTDevice; i++)
	{
		IoT_room_allocation[i] = EEPROM.read(room_allocation+i);
		IoT_device_type[i] = EEPROM.read(device_type+i);
	}
}

//====function====



//====HTML====
String defineIndex()
{
	String pageIndex = "";
    int count = 0;
    pageIndex = "<!DOCTYPE html>";
    pageIndex += "<html><head>";
    pageIndex += getJS(3);
    pageIndex += "</head>";
    pageIndex += "<body>";
    pageIndex += "<div class='index-box'>";
    pageIndex += "<div id='bulb' class='lightbulb-on-li'></div>";
    pageIndex += "<hr style='border: 1px solid transparent;'>";
    pageIndex += "<div id='wire3' style='transition: 0.2s ease-in'></div>";
    pageIndex += "<table id='index-table' style='transition: 0.3s ease-in;'>";
    pageIndex += "<tbody>";
    pageIndex += "<tr>";
    pageIndex += "<td id='menu-box' style='border-right: 3px solid peru; transition: 0.3s ease-in;' rowspan='2'>";
    pageIndex += "<a class='index' href = '/addDevice' style='background-color:#1982c4;' onmouseout='noLight();' onmouseover='blueLight();'>Assign IoT Device</a><br><br><br>";
    pageIndex += "<a class='index' href = '/viewDeviceRoom' style='background-color:#1a936f;' onmouseout='noLight();' onmouseover='greenLight();'>Device Details</a><br><br><br>";
    pageIndex += "<a class='index' href = '/changePW' style='background-color:#ffb400;' onmouseout='noLight();' onmouseover='yellowLight();'>Change Password</a><br><br><br>";
    pageIndex += "<a class='index' href = '/logout' style='background-color:#db3a34;' onmouseout='noLight();' onmouseover='redLight();'>Logout</a>";
    pageIndex += "</td>";
    pageIndex += "<td id='add-box' style='text-align: left; border-bottom: 3px solid peru; transition: 0.3s ease-in;' colspan='2'>";
    pageIndex += "<form action = '/addControlledDevice' method = 'post'>";
    pageIndex += "<b>Add Control Device Host ID:</b>";
    pageIndex += "<br> <span style='line-height: 3;'></span>";
    pageIndex += "<input type = 'number' name = 'Device' min='2' max='253'>&nbsp;";
    pageIndex += "<br> <span style='line-height: 1.5;'></span>";
    pageIndex += "<input type = 'submit' class='add' value = '&plus; Add Device'>";
    pageIndex += "</form>";
    pageIndex += "</td>";
    pageIndex += "</tr>";
    pageIndex += "<tr>";
    pageIndex += "<td colspan='2'>";
    pageIndex += "<form action = '/removeControlledDevice' method = 'post'>";
    pageIndex += "<table style='border:none; margin: 0; width:100%; height:200px; padding: 10px; text-align: left;'>";
    pageIndex += "<tr>";
    pageIndex += "<td style='padding: 0 0 5px 0;' colspan='2'><b>Existing Control Device(s):</b></td>";
    pageIndex += "</tr>";
    if(countControlledDevice == 0)
    {
      pageIndex += "<tr>";
      pageIndex += "  <td style='padding: 3px 0; border-bottom: 2px dotted black;'>No control device connected</td>";
      pageIndex += "</tr>";
    }
    else
    {
      for(int i = 0; i < countControlledDevice; i++)
      {
        if(EEPROM.read(EEPROM_CD[i])==255)
          continue;
        else
        {
          count++;
          pageIndex += "<tr>";
          pageIndex += "  <td style='padding: 3px 0; border-bottom: 2px dotted black;'>" + String(count) + ". Host ID: " + String(EEPROM.read(EEPROM_CD[i])) + "</td>";
          pageIndex += "  <td style='padding: 5px 0;'><input class='remove' type = 'submit' name = '"+ String(EEPROM_CD[i]) +"' value = '&#9668; Remove'></td>";
          pageIndex += "</tr>";
        }
      }
    }
//	if(updateSuccess)
//	{
//		pageIndex += "device added successfully";
//		updateSuccess = false;
//	}
    pageIndex += "</table>";
    pageIndex += "</form>";
    pageIndex += "</td>";
    pageIndex += "</tr>";
    pageIndex += "</tbody>";
    pageIndex += "</table>";
    pageIndex += "<br><br><br>";
    pageIndex += "</div>";
    pageIndex += "</body>";
    pageIndex += "</html>";
    return pageIndex;
}

String loginPage(bool LoginFail)
{
	String pageLogin = "";
	pageLogin = "<!DOCTYPE html>";
	pageLogin += "<html>";
  pageLogin += "<head>";
  pageLogin += "<title>Login Page</title>";
  pageLogin += getJS(1);
  pageLogin += "</head>";
  pageLogin += "<body>";
  pageLogin += "<div class='box'>";
  pageLogin += "<div id='bulb' class='lightbulb-off-li'></div>";
  pageLogin += "<hr id='wire1'>";
  pageLogin += "<form action = '/verify' method = 'post'>";
  pageLogin += "<table>";
  pageLogin += "<thead>";
  pageLogin += "<tr>";
  pageLogin += "  <th>Login</th>";
  pageLogin += "</tr>";
  pageLogin += "</thead>";
  pageLogin += "<tbody>";
  pageLogin += "<tr>";
  pageLogin += "  <td>Enter Password:</td>";
  pageLogin += "</tr>";
  pageLogin += "<tr>";
  pageLogin += "  <td><b>&plus; <input type = 'password' name = 'loginPW' id = 'loginPW' placeholder = '&#128498;' pattern = '[0-9]{6}' title = 'Enter 6 digit password' required> &minus;</b></td>";
  pageLogin += "</tr>";
  pageLogin += "<tr>";
  pageLogin += "  <td><br>";
  if(LoginFail)
    pageLogin += "<span style='color: red; font-family: Calibri; font-size: 16px;'>Wrong Password </span> <br><br>";
  pageLogin += "<br><a href = '/changePW'>Change Password?</a><br><br><br></td>";
  pageLogin += "</tr>";
  pageLogin += "</tbody>";
  pageLogin += "</table>";
  pageLogin += "<hr id='switch-li' class='switch-off-li'>";
  pageLogin += "<br><br>";
  pageLogin += "<div id='bullet' class='bullet-off-li'>&#8226;<input class='login-switch' type='submit' value='[LOGIN]' onmouseout='switchOff_li();' onmouseover='LoginSwitchOn();'>&#8226;</div>";
  pageLogin += "</form>";
  pageLogin += "</div>";
  pageLogin += "</body>";
	pageLogin += "</html>";
  return pageLogin;
}

String passwordPage(bool changeFail)
{
	String pagePW = "";
  pagePW = "<!DOCTYPE html>";
  pagePW += "<html>";
  pagePW += "<head>";
  pagePW += getJS(2);
  pagePW += "</head>";
  pagePW += "<body>";
  pagePW += "<div class='box'>";
  pagePW += "<div id='bulb' class='lightbulb-off-cp'></div>";
  pagePW += "<hr id='wire1'>";
  pagePW += "<hr id='wire2'>";
  pagePW += "<form action = '/updatePassword' method = 'post'>";
  pagePW += "<table>";
  pagePW += "<thead>";
  pagePW += " <tr>";
  pagePW += "   <th>Change Password</th>";
  pagePW += " </tr>";
  pagePW += "</thead>";
  pagePW += "<tbody>";
  pagePW += " <tr>";
  pagePW += "   <td>Enter password:</td>";
  pagePW += " </tr>";
  pagePW += " <tr>";
  pagePW += "   <td><b>&plus; <input type = 'password' id = 'currentPW' name = 'currentPW' placeholder = '&#128498;' pattern = '[0-9]{6}' title = 'Enter 6 digit password' required> &minus;</b></td>";
  pagePW += " </tr>";
  pagePW += " <tr>";
  pagePW += "   <td>Enter new password:</td>";
  pagePW += " </tr>";
  pagePW += "<tr><td>";
  pagePW += "    <b>&minus; <input type = 'password' id = 'newPW' name = 'newPW' placeholder = '&#128498;' pattern = '[0-9]{6}' title = 'Enter 6 digit password' required> &plus;</b>";
  if(changeFail)
    pagePW += "    <br><br><span style='color: red; font-family: Calibri; font-size: 16px;'>Current password incorrect.</span>";
  pagePW += "    <br><br><br>";
  pagePW += "</td></tr>";
  pagePW += "</tbody>";
  pagePW += "</table>";
  pagePW += "<hr id='switch-cp' class='switch-off-cp'>";
  pagePW += "<div id='bullet' class='bullet-off-cp'>&#8226;<input type = 'submit' class='password-switch' value = '[CHANGE PASSWORD]' onmouseout='switchOff_cp();' onmouseover='PasswordSwitchOn();'>&#8226;</div>";
  pagePW += "<br><br><br>";
  pagePW += "</form>";
  pagePW += "</div>";
  pagePW += "</body>";
	pagePW += "</html>";

  return pagePW;
}

String addDevicePage()
{
	int deviceCount;
	int roomCount;
	String pageAD = "";
	pageAD = "<!DOCTYPE html><html>";
	pageAD +="<head>";
	pageAD +="<title>Add Device Page</title>";
	pageAD += getJS(4);
	pageAD +="</head>";
	pageAD += "<body>";
	pageAD += "<div class='index-box'>";
	pageAD += "<div id='bulb' class='lightbulb-off-ad'></div>";
	pageAD += "<div id='wire4'></div>";
	pageAD += "<form action = '/addDevice' method = 'post'>";
	pageAD += "<table border='0'>";
	pageAD += "<tr>";
	pageAD += "<th colspan='3'>Add Devices</th><br>";
	pageAD += "</tr>";
	pageAD += "<tr>";
	pageAD += "<td style='text-align: center;' colspan='2'>Number of devices: </td>";
	pageAD += "<td style='text-align: left;'>";
	pageAD += "<input type = 'number' name = 'deviceCount' min = '1' max = '9' title = 'Enter number between 1 - 9'>";
	pageAD += "</td>";
	pageAD += "</tr>";
	pageAD += "<tr>";
	pageAD += "<td style='text-align: center;' colspan='2'>Number of rooms: </td>";
	pageAD += "<td style='text-align: left;'><input type = 'number' name = 'roomCount' min = '1' max = '9' title = 'Enter number between 1 - 9'></td>";
	pageAD += "</tr>";
	pageAD += "<tr>";
	pageAD += "<td colspan='3'>";
	pageAD += "<hr id='switch-ad1' class='switch-off-ad1'>";
	pageAD += "<div id='bullet' class='bullet-off-ad1'>&#8226;<input class = 'update-switch' type = 'submit' value = '[UPDATE]' onmouseout='switchOffAD1();' onmouseover='switchOnAD1();'>&#8226;</div>";
	pageAD += "<br></td>";
	pageAD += "</tr>";
	pageAD += "</form>";
	if(server.hasArg("deviceCount"))
	{
		deviceCount = server.arg("deviceCount").toInt();
		EEPROM.write(IoT_device_count, deviceCount);
	}
	else
		deviceCount = 0;
	
	if(server.hasArg("roomCount"))
	{
		roomCount = server.arg("roomCount").toInt();
		EEPROM.write(room_count, roomCount);
	}
	else
		roomCount = 0;
		
	if(deviceCount > 0 && roomCount > 0)
	{	
		pageAD += "<form action = '/initialized' method = 'post'>";
		pageAD += "<tr>";
		pageAD += "<th colspan='3'>Initialize Room of IoT Device</th>";
		pageAD += "</tr>";
		for(int i = 0; i < deviceCount; i++)
		{
			pageAD += "<tr>";
			pageAD += "<td style='text-align: right;'>Device " + String((i+1)) + ":</td>";
			pageAD += "<td style='text-align: right;'>";
			pageAD += "<div class='custom-select-room'>";
			pageAD += "<select name = 'device"+String((i+1))+"'>";
			for(int j = 0; j < roomCount; j++)
			{
				pageAD += "<option value = '"+String((j+1))+"'>Room "+String((j+1))+"</option>";
			}
			pageAD += "</select>";
			pageAD += "</div>";
			pageAD += "</td>";
			pageAD += "<td style='text-align: center;'>";
			pageAD += "<div class='custom-select-type'>";
			pageAD += "<select name = 'type"+String((i+1))+"'>";
			pageAD += "<option value = '0'>LED</option>";
			pageAD += "<option value = '1'>FAN</option>";
			pageAD += "</select>";
			pageAD += "</div>";
			pageAD += "</td>";
			pageAD += "</tr>";
		}
		pageAD += "<tr>";
		pageAD += "<td colspan='3'><br></tr>";
		pageAD += "</tr>";
		pageAD += "</table>";
		pageAD += "<hr id='switch-ad2' class='switch-off-ad2'>";
		pageAD += "<br><br>";
		pageAD += "<div id='bullet' class='bullet-off-ad2'>&#8226;<input class = 'save-change-switch' type = 'submit' value = '[SAVE CHANGE]' onmouseout='switchOffAD2();' onmouseover='switchOnAD2();'>&#8226;</div>";
		pageAD += "</form>";
	}
	pageAD += "</div>";
	pageAD += "<br>";
	pageAD += "</body>";
	pageAD += "</html>";
	
  return pageAD;
}

String showDeviceRoomAllocation()
{
	//pageVD = page View Device
	String pageVD = "";
	pageVD = "<!DOCTYPE html><html>";
	pageVD += "<head>";
	pageVD += "<title>Device Details</title>";
	pageVD += getJS(5);
	pageVD += "</head>";
	pageVD += "<body>";
	pageVD += "<div class='index-box'>";
	pageVD += "<div id='bulb' class='lightbulb-on' style='color: #1a936f;'></div>";
	pageVD += "<table>";
	pageVD += "<tr>";
	pageVD += "	<th>Device Details</th>";
	pageVD += "</tr>";
	pageVD += "<tr>";
	pageVD += "<td>";
	pageVD += "<table class='view-table'>";
	pageVD += "<tr style='font-weight: bold; background-color: #1a936f; border-bottom: 2px solid black'>";
	pageVD += "<td>Device No.</td>";
	pageVD += "<td>Room No.</td>";
	pageVD += "<td>Type</td>";
	pageVD += "<td>Status</td>";
	pageVD += "</tr>";
	for(int i = 0; i < maxIOTDevice; i++)
	{
		if(IoT_room_allocation[i]==255)
			break;
		else
		{
			pageVD += "<tr>";
			pageVD += "	<td>"+String((i+1))+"</td>";
			pageVD += "	<td>"+String(IoT_room_allocation[i])+"</td>";
			//pageVD += "	<td>"+((IoT_device_type[i]==0)? "LED":"FAN")+"</td>";
      pageVD += " <td>"+((IoT_device_type[i]==0)? String("LED"):String("FAN"))+"</td>";
			pageVD += "	<td>"+((IoT_state[i])? String("ON"):String("OFF"))+"</td>";
			pageVD += "</tr>";
		}
	}
	pageVD += "</table>";
	pageVD += "</td>";
	pageVD += "</tr>";
	pageVD += "<tr>";
	pageVD += "<td>";
	pageVD += "<a href = '/addDevice'>Add device?</a>&nbsp;";
	pageVD += "<a href = '/main'>Back</a><br>";
	pageVD += "</td>";
	pageVD += "</tr>";
	pageVD += "</table>";
	pageVD += "<br><br><br>";
	pageVD += "</div>";
	pageVD += "</body>";
	pageVD += "</html>";
	return pageVD;
}

//====JS====
void setSession(String label, String state, String nextLocation)
{
	String s = "<!DOCTYPE html>";
	s += "<html>";
	s += "<head>";
	s += "<script>";
	s += "if(sessionStorage.getItem('"+label+"') == null){";
	s += "sessionStorage.setItem('"+label+"','"+state+"');";
	s += "location.replace('"+nextLocation+"');}";
	s += "else if(sessionStorage.getItem('"+label+"') == 'true'){";
	s += "location.replace('"+nextLocation+"');}";
	s += "</script>";
	s += "</head>";
	s += "</html>";
	server.send(200, "text/html", s);
}

void clearSession(String itemKey, String nextLocation)
{
	String s = "<!DOCTYPE html>";
	s += "<html>";
	s += "<head>";
	s += "<script>";
	s += "sessionStorage.removeItem('"+itemKey+"');";
	s += "location.replace('"+nextLocation+"');";
	s += "</script>";
	s += "</head>";
	s += "</html>";
	server.send(200, "text/html", s);
}

//JSON
//construct JSON string

String createJSONtext()
{
	String text;
	
	text = "{\"house\":{";
	text += "\"room\":[";
	
	//room count
	for(int i = 0; i < EEPROM.read(room_count); i++)
	{
		text += "{\"roomNum\":"+String((i+1))+",";
		text += "\"device\":[";
		
		//loop for all device
		for(int j = 0; j < EEPROM.read(IoT_device_count); j++)
		{
			if(IoT_room_allocation[j] != (i+1))
			{
				continue;
			}
			
			text += "{\"deviceNum\":"+String((j+1))+",";	
			text += "\"type\":"+String((IoT_device_type[j]==0)? "\"LED\",":"\"FAN\",");
			text += "\"status\":"+ String((IoT_state[j])? "\"ON\"},":"\"OFF\"},");
		}
		text = text.substring(0,(text.length()-1)); //remove the last comma
		text += "]},";
	}
	text = text.substring(0,(text.length()-1)); //remove the last comma
	text += "]}}";
	
	return text;
}

void setup() {
  
  //defineIndex();
  //setup serial monitor
  Serial.begin(9600);
  //connect esp to wifi
  WiFi.begin(ssid, password);
  delay(10);
  //set up EEPROM
  EEPROM.begin(512);
  //for setting up eeprom purpose;
    //EEPROM.write(IoT_device_count,0);
    //EEPROM.write(room_count,0);
    //EEPROM.commit();
  //==============================
  
  //initialize EEPROM ADDRESS and PINMODE
  if(EEPROM.read(IoT_device_count)>0 && EEPROM.read(room_count)>0)
  {
	  init_EEPROM_CD();
	  init_default_password();
	  init_IoT_EEPROM_address();
	  init_pinMode();
	  init_deviceRoom_nType();
  }
  //==============================
  
  Serial.print("default value of address 36: ");
  Serial.println(EEPROM.read(36));
  //state_LED1 = EEPROM.read(EEPROM_LED1);	//need algorithm	set array	//done
  //state_LED2 = EEPROM.read(EEPROM_LED2);	//need algorithm	set array	//done
  
  //set pin mode
  //pinMode(ledPin0, OUTPUT);	//need algorithm	set for loop			//done
  //pinMode(ledPin1, OUTPUT);	//need algorithm	set for loop			//done
  
  //need to read from ESP EEPROM
  //to control the initial state
  //digitalWrite(ledPin0, state_LED1);	//need algorithm	set for loop	//done
  //digitalWrite(ledPin1, state_LED2);	//need algorithm	set for loop	//done
  
  //Configure the IP of the WiFi network
  //disconnect wifi first
  WiFi.disconnect();
  //set device name
  WiFi.hostname("myIOT");
  //set IP, gateway, subnet 
  WiFi.config(ip, gateway, subnet);
  //reconnect the network with configured IP
  WiFi.begin(ssid, password);

  //connecting
  Serial.println();
  Serial.println();
  Serial.print("Connecting to ");
  Serial.println(ssid);

  while (WiFi.status() != WL_CONNECTED) 
  {
    delay(500);
    Serial.print(".");
  }

  Serial.println("");
  Serial.println("WiFi connected");

  //debug..


  // Start the server
  server.on("/", login);
  server.on("/main", mainIndex);
  server.on("/verify", verifyPW);
  server.on("/logout", logout);
  server.on("/changePW", changePW);
  server.on("/updatePassword", updatePW);
  server.on("/addDevice", addDevice);
  server.on("/viewDeviceRoom", viewDeviceRoom);
  server.on("/removeControlledDevice", removeControlledDevice);
  server.on("/addControlledDevice", addControlledDevice);
  server.on("/resetControlledDevice", resetControlledDevice);
  server.on("/initialized", initializedDevice);
  server.on("/LED1ON", LED1ON);
  server.on("/LED1OFF", LED1OFF);
  server.on("/LED2ON", LED2ON);
  server.on("/LED2OFF", LED2OFF);
  
  //collect header..?
  //here the list of headers to be recorded
  const char * headerkeys[] = {"ON","OFF"} ;
  size_t headerkeyssize = sizeof(headerkeys)/sizeof(char*);
  //ask server to track these headers
  server.collectHeaders(headerkeys, headerkeyssize );
  
  server.begin();
  Serial.println("Server started");
  
  // Print the IP address
  Serial.print("Use this URL to connect: ");
  Serial.print("http://");
  Serial.print(WiFi.localIP());
  Serial.println("/");
}
//============page handling============//
void login()
{
  Serial.println("root called;");
  Serial.print("No. Header: ");
  Serial.println(server.headers());

  for(int i = 0; i < server.headers(); i++)
  {
    Serial.print("name: ");
    Serial.println(server.headerName(i));  
    Serial.print("value: ");
    Serial.println(server.header(server.headerName(i)));
  }
	//change the target name	//do loping maybe :/
	
  if(server.hasHeader("ON"))
  {
    Serial.println("ON detected");
    if(verifyControlledDevice())
    {
	    deviceNum = server.header("ON").toInt();
	    if(!IoT_Flag_state[deviceNum])
	    {		
  	  	IoT_Flag_state[deviceNum] = true;
     		digitalWrite(IoT_IO[deviceNum], HIGH);
    		EEPROM.write(IoT_EEPROM_address[deviceNum], HIGH);
     		EEPROM.commit();
   	  	IoT_state[deviceNum] = true;
   		  IoT_Flag_state[deviceNum] = false;
        Serial.println(deviceNum + " ON.");
    		server.send(200, "text/html", "ok");
  	  }
  	  else
  	  {
  	  	server.send(503, "text/html", "no");
  	  }
    }
    else
    {
        server.send(203, "text/html", "no");
    }
  }	  
  else if(server.hasHeader("OFF"))
  {
    Serial.println("OFF detected");
    if(verifyControlledDevice())
    {
  	  deviceNum = server.header("OFF").toInt();
  	  if(!IoT_Flag_state[deviceNum])
  	  {
  	  	IoT_Flag_state[deviceNum] = true;
  	  	digitalWrite(IoT_IO[deviceNum], LOW);
  		  EEPROM.write(IoT_EEPROM_address[deviceNum], LOW);
  		  EEPROM.commit();
  		  IoT_state[deviceNum] = false;
  		  IoT_Flag_state[deviceNum] = false;
		    Serial.println(deviceNum + " OFF.");
  		  server.send(200, "text/html", "ok");
  	  }
  	  else
  	  {
  		  server.send(503, "text/html", "no");
  	  }
     }
    else
    {
        server.send(203, "text/html", "no");
    }
  }	  
  else
  {
    String s = loginPage(false);
	  server.sendHeader("LED0", String(EEPROM.read(IoT_EEPROM_address[0])));
	  server.sendHeader("LED1", String(EEPROM.read(IoT_EEPROM_address[1])));
	  server.sendHeader("json", createJSONtext());
	  server.send(200, "text/html", s);
  }
  
  Serial.println(createJSONtext());
  
}

void logout()
{
  clearSession("login", rootIP);
}

void mainIndex()
{
  String s = defineIndex();
  delay(500);
  server.send(200, "text/html", s);  
}

void changePW()
{
  String s = passwordPage(false);
  delay(500);
  server.send(200, "text/html", s);  
}

void addDevice()
{
  String s = addDevicePage();
  delay(500);
  server.send(200, "text/html", s);	
}

void viewDeviceRoom()
{
  String s = showDeviceRoomAllocation();
  delay(500);
  server.send(200, "text/html", s);	
}

//=====page handling with operation=====//
//EEPROM read and write
void verifyPW()
{
	//add verify operation here
  String input = server.arg("loginPW");
  //Serial.print("input password: ");
  //Serial.println(input);
  bool isMatch = true;
  //Serial.println("what char at see...");
  for(int i = 0; i < passwordLength; i++)
  {
      //Serial.println(input.charAt(i)- '0');
      //Serial.println(EEPROM.read(EEPROM_password_address+i));
      //Serial.println(int(input.charAt(i)- '0'));
    if(int(input.charAt(i)- '0') != EEPROM.read(EEPROM_password_address+i))
	  {
      isMatch = false;
		  break;
	  }
  }
  if(isMatch)
  {
	  setSession("login", "true", rootIP+"main");
  }
  else
  {
    //String s = "Wrong password";
    //s += loginPage();
    String s = loginPage(true);
    server.send(200, "text/html", s);
  }
}

void updatePW()
{
  //add update password operation here
  String currentPW = server.arg("currentPW");
  String newPW = server.arg("newPW");
  bool isMatch = true;
  
  for(int i = 0; i < passwordLength; i++)
  {
	  if(int(currentPW.charAt(i) - '0') != EEPROM.read(EEPROM_password_address+i))
	  {
	  	isMatch = false;
	  	break;
	  }
  }	  
  
  if(isMatch)
  {
	  for(int i = 0; i < passwordLength; i++)
	  {
	  	EEPROM.write(EEPROM_password_address+i, int(newPW.charAt(i) - '0'));
	  }
	  //EEPROM.write(EEPROM_password_change_status, 0);
	  EEPROM.commit();
	
	  String s = "Password changed successfully. Please log in with new password :)";
	  s += loginPage(false);
	  server.send(200, "text/html", s);
  }
  else
  {
	  //String s = "Current password incorrect.";
	  //s += passwordPage(true);
	  String s = passwordPage(true);
	  server.send(200, "text/html", s);
  }
}

void removeControlledDevice()
{
  Serial.println(server.argName(0));
  int address = server.argName(0).toInt() - 10;
  countControlledDevice--;
  int count = countControlledDevice;

  while(address < count)
  {
    EEPROM.write(EEPROM_CD[address], EEPROM.read(EEPROM_CD[address+1]));
    address++;  
  }

  if(address == count)
    EEPROM.write(EEPROM_CD[address],255);

  mainIndex();
}

void addControlledDevice()
{
  Serial.println("adding");
  Serial.println("countControlledDevice" + String(countControlledDevice));
  if(countControlledDevice < maxControlledDevice)
  {
    for(int i = 0; i < maxControlledDevice; i++)
    {
      Serial.println("checking eeprom address");
      if(EEPROM.read(EEPROM_CD[i]) == 255)
      {
        EEPROM.write(EEPROM_CD[i], server.arg("Device").toInt());
        countControlledDevice++;
        Serial.println("added");
        break;
      }
    }
	  EEPROM.commit();
  }
  mainIndex();
}

void resetControlledDevice()
{
  for(int i = 0; i < maxControlledDevice; i++)
  {
    EEPROM.write(EEPROM_CD[i], 255);
  }  
  countControlledDevice = 0;
  EEPROM.commit();
  mainIndex();
}

void initializedDevice()
{
	Serial.println("No. Arg: " + String(server.args()));
	if(server.args() > 0)
	{
		/*
		for(int i = 0; i < server.args()-1; i++)
		{
			Serial.println("arg name: " + server.argName(i));
			Serial.println("arg value: " + server.arg(i));
			
		}
		*/
		for(int i = 0; i < maxIOTDevice; i++)
		{
			if(i < (server.args()-1)/2)
			{
				EEPROM.write(room_allocation+i, server.arg((i*2)).toInt());
				EEPROM.write(device_type+i, server.arg((i*2)+1).toInt());
			}
			else
			{
				EEPROM.write(room_allocation+i, 255);
				EEPROM.write(device_type+i, 255);
			}
		}
	}
  EEPROM.commit();
  logout();
  delay(1000); 
  ESP.reset();
}

//==========pinout trigger=============// 192.168.1.200/LED1ON
void LED1ON()
{  
  if(verifyControlledDevice())
  {
	  digitalWrite(IoT_IO[0], HIGH);
	  EEPROM.write(IoT_EEPROM_address[0], HIGH);
	  EEPROM.commit();
	  IoT_Flag_state[0] = false;
    Serial.println("LED1 is free");
	  Serial.println("LED1 ON");
	  server.send(200, "text/html", "ok");
	  Serial.println("LED1 ON done");
  }
  else
  {
	  Serial.println("device don't have access to do so.");
  }
}

void LED1OFF()
{
  if(verifyControlledDevice())
  {
	digitalWrite(IoT_IO[0], LOW);
	EEPROM.write(IoT_EEPROM_address[0], LOW);
	EEPROM.commit();
	IoT_Flag_state[0] = false;
  Serial.println("LED1 is free");
	server.send(200, "text/html", "ok");
  }
  else
  {
	Serial.println("device don't have access to do so.");
  }
}

void LED2ON()
{
  digitalWrite(IoT_IO[1], HIGH);
  EEPROM.write(IoT_EEPROM_address[1], HIGH);
  EEPROM.commit();
  IoT_Flag_state[1] = false;
  Serial.println("LED2 is free");
  server.send(200, "text/html", "ok");
}

void LED2OFF()
{
  digitalWrite(IoT_IO[1], LOW);
  EEPROM.write(IoT_EEPROM_address[1], LOW);
  EEPROM.commit();
  IoT_Flag_state[1] = false;
  Serial.println("LED2 is free");
  server.send(200, "text/html", "ok");
}

bool verifyControlledDevice()
{
	int hostID_index = 10;//192.168.1.x
	String ip = server.client().remoteIP().toString();
	int client_hostID = ip.substring(hostID_index).toInt();
	bool with_Access = false;
	
	Serial.print("client's IP: ");
	Serial.println(ip);
	Serial.print("client's host ID: ");
	Serial.println(client_hostID);
	
	for(int i = 0; i < countControlledDevice; i++)
	{
		if(client_hostID == EEPROM.read(EEPROM_CD[i]))
		{
			with_Access = true;
			break;
		}
	}
	return with_Access;
}

void loop() 
{
  server.handleClient();
  //pageIndex = "";  
  //pageLogin = "";
  //pagePW = "";
  //pageAD = "";
  //pageVD ="";
}
