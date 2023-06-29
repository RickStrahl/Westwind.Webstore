import {createApp} from "/lib/vue/dist/vue.esm-browser.js";


// this will be the bindable proxy - not model!
var vm = null;

    createApp({
        data () {
            var model = {
                lineItem: {},
                initialize: function() {
                    model.lineItem = globals.lineItem;
                },
            }
            
            model.initialize();

            return model;
        },
        methods: {
            recalculate() {
                ajaxJson("/admin/ordermanager/api/lineitem/recalculate",
                    vm.lineItem,
                    function (li) {
                        vm.lineItem.itemTotal = li.itemTotal;
                    },
                    function (error) {                     
                        toastr.error("Lineitem recalculate failed." + error.message);
                    });
            },
            saveLineItem() {
           
                return ajaxJson("/admin/ordermanager/api/lineitem",
                    vm.lineItem,
                    function(li) {
                        window.location = "/admin/ordermanager/" + li.invoiceId;
                    },
                    function(error) {
                        toastr.error("Lineitem saving failed: " + error.message);
                    });
            },
            
            
            // Licensing
            
            createLicense() {
                if (vm.lineItem.licenseSerial && 
                    !confirm("Are you sure you want to create a *new* license?"))
                    return;

                ajaxJson("/admin/ordermanager/api/lineitem",
                    vm.lineItem,
                    function(li) {
                        ajaxJson("/admin/ordermanager/api/licensing/create/" + vm.lineItem.invoiceId + "/" + vm.lineItem.id,
                            null,
                            function (result) {
                                vm.lineItem.licenseSerial = result.data.licenseSerial;
                                toastr.success("A new license has been created.")
                            },
                            function (error) {
                                toastr.error(error.message);
                            });
                    },
                    function(error) {
                        toastr.error("Lineitem saving failed: " + error.message);
                    });
            },
            revokeLicense() {
                if (!confirm("Are you sure you want to revoke this license?"))
                    return;
                
                ajaxJson("/admin/ordermanager/api/licensing/revoke/" + vm.lineItem.invoiceId + "/" + vm.lineItem.id,
                    null,
                    function(result) {
                        toastr.success("License has been revoked.")
                    },
                    function(error) {
                        toastr.error(error.message);
                    });
            },
            reissueLicense() {
                if (!confirm("Are you sure you want to reissue this license?"))
                    return;

                ajaxJson("/admin/ordermanager/api/licensing/reissue/" + vm.lineItem.invoiceId + "/" + vm.lineItem.id,
                    null,
                    function(result) {
                        vm.lineItem.licenseSerial = result.data.licenseSerial;
                        toastr.success("License has been reissued.")
                    },
                    function(error) {
                        toastr.error(error.message);
                    });
            },
            
        },
        mounted() {
           vm = this;
               var $sku = $("#Sku");
                             
               // Hook up search box
               if ($sku.length > 0) {
                   $sku.typeahead({
                       source: [],
                       autoselect: true,
                       items: 10,
                       fitToElement: false,
                       delay: 200,
                       minLength: 0,
                       afterSelect: function (item) {
                           $sku.data('typeahead').lastSelection = item;
                           var sku = item.id;
                           ajaxJson("/admin/ordermanager/api/product/" + encodeURIComponent(sku), null, 
                               function(product) {
                                    
                                    vm.lineItem.sku = product.sku;
                                    vm.lineItem.description = product.description;
                                    vm.lineItem.price = product.price;
                                    vm.lineItem.quantity = 1;
                                    vm.lineItem.discountPercent = product.discountPercent;
                                    vm.lineItem.itemImage = product.itemImage;
                                    
                                    vm.lineItem.autoRegister = product.autoRegister;
                                    vm.lineItem.useLicensing = product.useLicensing;
                                    
                                    console.log(vm.lineItem, product);
                                    vm.recalculate();

                               });
                       }
                   });
                   $sku.keyup(searchProducts);
                   $sku.focus();
               }

               function searchProducts(e) {
                   var val = this.value;
                   console.log(val);
                   ajaxJson("/api/product/searchall/" + encodeURIComponent(this.value), null,
                       function (result) {
                           $sku.data('typeahead').source = result;
                       }, null, { method: "GET" });
               }
        }
    }).mount('#app');

