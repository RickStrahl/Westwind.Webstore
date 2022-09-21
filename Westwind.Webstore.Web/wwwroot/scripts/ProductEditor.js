editor = {};

// globals{} is passed in from main page
editor = {
    product: null,
    initialize: function() {
        editor.loadProduct();
    },
    loadProduct: function(sku = null) {
        if (!sku)
            sku = globals.sku;
        
        return ajaxJson("/admin/ordermanager/api/product/" + globals.sku,
            function(product) {
                page.product = product;
            },
            function(err) {
                toastr.error("failed to load product from api: " + err.message);
            }
        );
    },
    
    
}

$( function() {
    editor.initialize();
})