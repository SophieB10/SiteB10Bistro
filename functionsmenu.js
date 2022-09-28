
let totalPrice = parseFloat(0);
var dishlist = new Array();

function ListOrder() {

    var paragraph = document.createElement("li");

    var selecteddish = document.querySelector('input[name="order_item"]:checked').value;

    dishlist.push(selecteddish.substring(0, selecteddish.indexOf(":")));

    const price = String(selecteddish).split(" ").pop();

    totalPrice += parseFloat(price);

    var text = document.createTextNode(selecteddish);

    paragraph.appendChild(text); // Zet de text in de paragraph

    var element = document.getElementById("bonnetje");//Pak de target div

    element.appendChild(paragraph); // Voeg nieuwe paragraaf aan target div

    console.log(dishlist);
}

function totalAmount(){
    document.getElementById("price").innerHTML = "Totaal Bedrag: " + totalPrice.toFixed(2);
    var orderstring = dishlist.toString();
    console.log(orderstring);

    let BODY = {
        "priceCalculated": totalPrice.toFixed(2),
        "order": orderstring
       };


   //fetch( 'https://b10bc-weu-httptriggersophie-fa.azurewebsites.net/api/TableOutput', {
    fetch( 'http://localhost:7071/api/TableOutput', {
        method: 'POST',
        headers: {  
            'Content-Type': 'application/json'  
        },  
        body: JSON.stringify(BODY)
    })
    .catch((error) => {
        console.log(error);
    });

    var btn1 = document.getElementById("btn1");
    btn1.style.display = 'none';
    var btn2 = document.getElementById("btn2");
    btn2.style.display = 'none';
}
