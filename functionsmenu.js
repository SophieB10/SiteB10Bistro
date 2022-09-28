
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
    let requestBody = JSON.stringify(BODY);

    

   fetch( 'https://b10bc-weu-httptriggersophie-fa.azurewebsites.net/api/TableOutput', {
    method: 'POST',
    headers: {  
        'Content-Type': 'application/json'  
      },  
    Body: requestBody
    })
    .then(res => {
        if(!res.ok) {
          return res.text().then(text => { throw new Error(text) })
         }
        else {
         return res.json();
       }    
      })
    .catch((error) => {
        console.log(error);
    });

    var btn1 = document.getElementById("btn1");
    btn1.style.display = 'none';
    var btn2 = document.getElementById("btn2");
    btn2.style.display = 'none';
}
