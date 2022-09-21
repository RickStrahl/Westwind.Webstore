window.webstore = {};

var $input = null;

webstore = {
    initialize: function () {
        $input = $('#ProductSearchBox');

        // toggle menu, handle menu click and outside click to close
        $(document).on("click",
            ".slide-menu-toggle-open,.slide-menu-toggle-close," +
            ".slide-menu a, #SamplesLink,.slide-menu",
            function() {
                $(".slide-menu").toggleClass("active");
            });
        // close when clicking outside of .slide-menu
        $(document).on("click",
            "#MainView",
            function() {
                var $sm = $(".slide-menu");
                if ($sm.hasClass("active"))
                    $sm.removeClass("active");
            });
        $("#btnSamplesLink").click(function() {
            $(".slide-menu").toggleClass("active");
            return false;
        });

        // Hook up search box
        if ($input.length > 0) {
            $input.typeahead({
                source: [],
                autoselect: true,
                items: 10,
                fitToElement: false,
                delay: 200,
                minLength: 0,
                afterSelect: function(item) {
                    $input.data('typeahead').lastSelection = item;
                    window.location = "/product/" + encodeURIComponent(item.id);
                }
            });
            $input.keyup(webstore.searchProducts);
            $input.focus();
        }
    },
    searchProducts: function searchProducts(e) {
        var val = this.value;

        if (!val) {
            webstore.clearSearchProducts();
            return;
        }

        ajaxJson("/api/product/search/" + encodeURIComponent(this.value), null,
            function (result) {
                $input.data('typeahead').source = result;
            }, null, { method: "GET" });
    },
    clearSearchProducts: function clearSearchProducts() {
        $input.val('');
        $input.data('typeahead').source = [];
    },
    dateKeys: function(ev) {
        var keyCode = ev.keyCode;
        if (keyCode == 84) {
            var dt = new Date();
            ev.srcElement.value =  dt.getFullYear() + "-" + (dt.getMonth() + 1).toString().padStart(2,"0")  + "-" + dt.getDate().toString().padStart(2,'0')
            ev.preventDefault();
            ev.cancelBubble = true;
            return false;
        }
    }
};

$(function () {
    webstore.initialize();
});
