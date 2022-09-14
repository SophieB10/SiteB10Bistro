let menu;

function getJSON(){
    fetch('https://b10bc-weu-httptriggersophie-fa.azurewebsites.net/api/HelloWorld', {method: "GET"})
        .then((response)=> response.json())
        .then((data)=>{
            menu=data;
            console.log(menu);
            printPrice(menu);
            }
             )
        .catch((error)=>{console.log(error)});   
}

function printPrice(file){
    const radioButton = document.getElementById("radiobutton")
    file.forEach(Dishes =>{
        let input = document.createElement("input");
        input.type = "radio";
        input.value = Dishes.Dish + ": " + Dishes.Price;
        input.name = "order_item";
        input.id = Dishes.Dish;
        let label = document.createElement("div");
        label.innerHTML = Dishes.Dish + ": " + Dishes.Price;
        label.appendChild(input);
        radioButton.appendChild(label);
    })  
}



