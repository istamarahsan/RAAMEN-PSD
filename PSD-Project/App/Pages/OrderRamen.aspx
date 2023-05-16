<%@ Page Language="C#" CodeBehind="OrderRamen.aspx.cs" Inherits="PSD_Project.App.Pages.OrderRamen" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Order</title>
    <script>
    const ramen = {
        <% foreach (var ramen in Ramen)
           { %>
           <%= ramen.Id %> : {
            '<%= ramen.Name %>'
           },
        <% } %>
    }
    const cartItems = {}
    function onAddToCart(ramenId) {
      cartItems[ramenId] = (cartItems[ramenId] ?? 0) + 1
      const cart = document.getElementById('cart')
      for (const child of cart.children) {
        cart.removeChild(child)
      }
      for (const ramenIdKey in cartItems) {
      if (cartItems[ramenIdKey] <= 0) continue
        cart.appendChild(renderCartItem({
            name: ramen[ramenIdKey]['name'],
            quantity: cartItems[ramenIdKey]
        })) 
      }
    }
    
    function renderCartItem(props) {
        const container = document.createElement('div')
        const nameDiv = document.createElement('div')
        const quantityDiv = document.createElement('div')
        nameDiv.innerHTML=props['name'] ?? ''
        quantityDiv.innerHTML=props['quantity'] ?? ''
        container.appendChild(nameDiv)
        container.appendChild(quantityDiv)
        return container
    }
    
    function onClearCart() {
    const cart = document.getElementById('cart')
      for (const child of cart.children) {
              cart.removeChild(child)
    }
    for (const cartItemsKey in cartItems) {
      cartItems[cartItemsKey] = 0
    }
    }
    
    </script>
</head>
<body>
<div style="display: flex; flex-flow: row; flex-wrap: nowrap">
    <div style="display: flex; flex-flow: row; flex-wrap: wrap">
        <% foreach (var ramen in Ramen)
           { %>
            <div style="display: flex; flex-flow: column">
                <div><%= ramen.Name %></div>
                <div><%= ramen.Meat.Name %></div>
                <div><%= ramen.Borth %></div>
                <div>Rp. <%= ramen.Price %></div>
                <div id="quantity-<%= ramen.Id %>"></div>
                <button onclick="onAddToCart(<%= ramen.Id %>)">Add to Cart</button>
            </div>
        <% } %>
    </div>
    <div style="display: flex; flex-flow: column; justify-content: flex-start">
        <div style="display: flex; flex-flow: column" id="cart">

        </div>
        <button onclick="onClearCart()">Clear</button>
    </div>

</div>

</body>
</html>