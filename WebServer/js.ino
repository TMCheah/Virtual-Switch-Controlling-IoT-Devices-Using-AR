//page number reference
// login = 1
// change password = 2
// index = 3
// addDevice = 4
// displayDevice = 5
// 

String getJS(int page)
{
  String j = "";
  //CSS
  j+="<style>";
  j+="@import url('https://drive.google.com/uc?export=view&id=1lCq1TN8D3WxGGA3Y02nBlbEWp7nzMOJB');";
  j+="</style>";
  //JS
  j+="<script>";
  if(page == 3)
  {
    j+="if(sessionStorage.getItem('login') == null)";
    j+="location.replace('"+rootIP+"');";
  }
  j+="  function PasswordSwitchOn() {";
  j+="    if((document.getElementById('currentPW').validity.valid && document.getElementById('newPW').validity.valid)) {";
  j+="      document.getElementById('bulb').classList.replace('lightbulb-off-cp', 'lightbulb-on-cp');";
  j+="    document.getElementById('bulb').style.color=('#ffb400');";
  j+="      document.getElementById('switch-cp').classList.replace('switch-off-cp', 'switch-on-cp');";
  j+="      document.getElementById('bullet').classList.replace('bullet-off-cp', 'bullet-on-cp');";
  j+="    }";
  j+="    else switchOff_cp();";
  j+="  }";
  
  j+="  function LoginSwitchOn() {";
  j+="    if(document.getElementById('loginPW').validity.valid) {";
  j+="      document.getElementById('bulb').classList.replace('lightbulb-off-li', 'lightbulb-on-li');";
  j+="      document.getElementById('switch-li').classList.replace('switch-off-li', 'switch-on-li');";
  j+="      document.getElementById('bullet').classList.replace('bullet-off-li', 'bullet-on-li');";
  j+="    }";
  j+="    else switchOff_li();";
  j+="  }";
  
  j+="  function switchOff_cp() {";
  j+="    document.getElementById('bulb').classList.replace('lightbulb-on-cp', 'lightbulb-off-cp');";
  j+="    document.getElementById('switch-cp').classList.replace('switch-on-cp', 'switch-off-cp');";
  j+="    document.getElementById('bullet').classList.replace('bullet-on-cp', 'bullet-off-cp');";
  j+="  }";
  
  j+="  function switchOff_li() {";
  j+="    document.getElementById('bulb').classList.replace('lightbulb-on-li', 'lightbulb-off-li');";
  j+="    document.getElementById('switch-li').classList.replace('switch-on-li', 'switch-off-li');";
  j+="    document.getElementById('bullet').classList.replace('bullet-on-li', 'bullet-off-li');";
  j+="  }";
  
  j+="  function blueLight() {";
  j+="    document.getElementById('bulb').classList.replace('lightbulb-off', 'lightbulb-on');";
  j+="    document.getElementById('bulb').style.color=('#1982c4');";
  j+="    document.getElementById('index-table').style.border='3px solid #1982c4';";
  j+="    document.getElementById('menu-box').style.borderRight='3px solid #1982c4';";
  j+="    document.getElementById('add-box').style.borderBottom='3px solid #1982c4';";
  j+="    document.getElementById('wire3').style.borderLeft='3px solid #1982c4';";
  j+="}";
  
  j+="function greenLight() {";
  j+="    document.getElementById('bulb').classList.replace('lightbulb-off', 'lightbulb-on');";
  j+="    document.getElementById('bulb').style.color=('#1a936f');";
  j+="    document.getElementById('index-table').style.border='3px solid #1a936f';";
  j+="    document.getElementById('menu-box').style.borderRight='3px solid #1a936f';";
  j+="    document.getElementById('add-box').style.borderBottom='3px solid #1a936f';";
  j+="    document.getElementById('wire3').style.borderLeft='3px solid #1a936f';";
  j+="}";
  
  j+="function yellowLight() {";
  j+="    document.getElementById('bulb').classList.replace('lightbulb-off', 'lightbulb-on');";
  j+="    document.getElementById('bulb').style.color=('#ffb400');";
  j+="    document.getElementById('index-table').style.border='3px solid #ffb400';";
  j+="    document.getElementById('menu-box').style.borderRight='3px solid #ffb400';";
  j+="    document.getElementById('add-box').style.borderBottom='3px solid #ffb400';";
  j+="    document.getElementById('wire3').style.borderLeft='3px solid #ffb400';";
  j+="}";
  
  j+="function redLight() {";
  j+="    document.getElementById('bulb').classList.replace('lightbulb-off', 'lightbulb-on');";
  j+="    document.getElementById('bulb').style.color=('#db3a34');";
  j+="    document.getElementById('index-table').style.border='3px solid #db3a34';";
  j+="    document.getElementById('menu-box').style.borderRight='3px solid #db3a34';";
  j+="    document.getElementById('add-box').style.borderBottom='3px solid #db3a34';";
  j+="    document.getElementById('wire3').style.borderLeft='3px solid #db3a34';";
  j+="}";
  
  j+="function noLight() {";
//j+="    document.getElementById('bulb').classList.replace('lightbulb-on', 'lightbulb-off');";
  j+="    document.getElementById('bulb').style.color=('#fdea7b');";
  j+="    document.getElementById('index-table').style.border='3px solid peru';";
  j+="    document.getElementById('menu-box').style.borderRight='3px solid peru';";
  j+="    document.getElementById('add-box').style.borderBottom='3px solid peru';";
  j+="    document.getElementById('wire3').style.borderLeft='3px solid peru';";
  j+="}";
  
  j+="function switchOnAD1() {";
  j+="  document.getElementById('bulb').classList.replace('lightbulb-off-ad', 'lightbulb-on-ad');";
  j+="  document.getElementById('bulb').style.color=('#1982c4');";
  j+="  document.getElementById('switch-ad1').classList.replace('switch-off-ad1', 'switch-on-ad1');";
  j+="  document.getElementById('bullet').classList.replace('bullet-off-li', 'bullet-on-li');";
  j+="}";
  
  j+="function switchOffAD1() {";
  j+="  document.getElementById('bulb').classList.replace('lightbulb-on-ad', 'lightbulb-off-ad');";
  j+="  document.getElementById('switch-ad1').classList.replace('switch-on-ad1', 'switch-off-ad1');";
  j+="  document.getElementById('bullet').classList.replace('bullet-on-li', 'bullet-off-li');";
  j+="}";
  
  j+="function switchOnAD2() {";
  j+="  document.getElementById('bulb').classList.replace('lightbulb-off-ad', 'lightbulb-on-ad');";
  j+="  document.getElementById('bulb').style.color=('#1982c4');";
  j+="  document.getElementById('switch-ad2').classList.replace('switch-off-ad2', 'switch-on-ad2');";
  j+="  document.getElementById('bullet').classList.replace('bullet-off-li', 'bullet-on-li');";
  j+="}";
  
  j+="function switchOffAD2() {";
  j+="  document.getElementById('bulb').classList.replace('lightbulb-on-ad', 'lightbulb-off-ad');";
  j+="  document.getElementById('switch-ad2').classList.replace('switch-on-ad2', 'switch-off-ad2');";
  j+="  document.getElementById('bullet').classList.replace('bullet-on-li', 'bullet-off-li');";
  j+="}";
  j+="</script>";
  return j;
}
