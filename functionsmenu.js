
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
    var orderlist = dishlist.toString();
    console.log(orderlist);

    const options = {
        method: 'POST',
        headers: {   
            'Accept': 'application/json',   
            'Content-Type': 'application/json'  
          },  
         body: `{
            "priceCalculated": ${totalPrice.toFixed(2)},
            "Order": "${orderlist}"]
           }`,  
    };

   fetch( 'https://b10bc-weu-httptriggersophie-fa.azurewebsites.net/api/TableOutput?', options );
    var btn1 = document.getElementById("btn1");
    btn1.style.display = 'none';
    var btn2 = document.getElementById("btn2");
    btn2.style.display = 'none';
}
