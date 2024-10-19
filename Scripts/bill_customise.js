console.log("bill_customise initiated!! : " + Date());

function ChangeCompName(compName, systemName) {
    var heading = document.getElementById('logo_text');
    var headTxt = heading.getElementsByTagName('a');
    headTxt[0].firstChild.nodeValue = compName;
    headTxt[0].lastChild.innerText = " " + systemName;
    console.log("bill_customise  - ChangeCompName: Executed!" + Date.now.toString());
}

function ChangeSlogan(compSlogan) {
    var bannertxt = document.getElementById("logo_text");
    var slogan = bannertxt.getElementsByTagName("h2");
    slogan[0].innerText = compSlogan;

    console.log("bill_customise  - ChangeSlogan: Executed!" + Date.now.toString());
}

function AddLogoText(logoText) {
    RemoveLogoText();
    var div = document.createElement('div');
    div.innerHTML = logoText;
    div.setAttribute('class', 'logo_Text');
    RemoveLogoText();
    document.getElementById('logo_pos').appendChild(div);

    console.log("bill_customise  - AddLogoText: Executed!" + Date.now.toString());
}

function RemoveLogoText() {
    document.getElementById('logo_pos').innerHTML = "";

    console.log("bill_customise  - RemoveLogoText: Executed!" + Date.now.toString());
}

function ShowLogoImg() {
    RemoveLogoText();
    var img = document.createElement('img');
    img.setAttribute('id', 'imgMainLogo');
    img.setAttribute('src', '../Images/logo2.png');
    img.style.width = "100%";
    img.style.height = "100%";
    document.getElementById('logo_pos').style.backgroundColor = 'transparent';
    document.getElementById('logo_pos').appendChild(img);

    console.log("bill_customise  - ShowLogoImg: Executed!: " + Date.now.toString());
}

function ChangeSystemName(systemName) {
    //var sysName = document.getElementById('SystemName');
    //sysName.innerHTML = sysName;

    $("h1#SystemName").html(systemName);
    console.log("bill_customise  - ChangeSystemName: Executed!" + Date.now.toString());
}

function ChangeSystemSlogan(SystemSlogan) {
    //var sysName = document.getElementById('SystemSlogan');
    //sysName.innerHTML = sysName;

    $("p#SystemSlogan").html(SystemSlogan);
    console.log("bill_customise  - ChangeSystemSlogan: Executed!" + Date.now.toString());
}