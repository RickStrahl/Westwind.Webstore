page = {};

// globals{} is passed in from main page
page = {
    initialize() {
    },
    showNewItem(key) {
        $("#new").show();
        setTimeout(()=>   $("#key-new").focus(), 120);
        
        var id = 'new';
        
        if(key)
            $("#key-" + id).val(key);
        $("#cdata-" + id).val('');
        $("#cdata1-" + id).val('');
        $("#ndata-" + id).val('');

    },
    saveItem(id) {
        var lookup = {
            id: id,
            cdata: $("#cdata-" + id).val(),
            cdata1: $("#cdata1-" + id).val(),
            ndata: $("#ndata-" + id).val() * 1
        };
        if (id === 'new'){
            lookup.key = $("#key-new").val();
        }

        if (Number.isNaN(lookup.nData))
            number.ndata = 0;

        ajaxJson("/admin/lookupsmanager", lookup,
            ()=> {
                toastr.success("Item updated.");
                $("#new").hide();

                if (id === 'new') {
                    window.location.reload();
                }
            },
            (error)=> {
                toastr.error("Failed to update item: " + error.message);
            })
        
    },
    deleteItem(id) {
        if (!confirm("Are you sure you want to delete this item?"))
            return;
        
        ajaxJson("/admin/lookupsmanager/" + id,null,
        ()=> {
            var href = window.location.href;
            $("#" + id).remove();
        },
        (error)=> {
            toastr.error("Failed to delete item: " + error.message);
        }, { method: "DELETE" })
    }
}

$(function () {
    page.initialize();
})