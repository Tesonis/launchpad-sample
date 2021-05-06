// Multistep form navigation
function Step1() {
    $(".step").addClass("d-none");
    $("#step1").removeClass("d-none");
    $(".progressbar li").removeClass("active");
    $(".progressbar li").eq(0).addClass("active");
    scrolltotop();
}
function Step2(page, action) {
    if (page == "step1" && action == "create") {
        GetStep2Data('create');
    }
    else if (page == "step1" && action == "update") {
        GetStep2Data('update');
        
        ListLaunchStatus();
    }
    else if (page == "step1" && action == "cdmupdate") {
        GetStep2Data('cdm');
    }
    $(".step").addClass("d-none");
    $("#step2").removeClass("d-none");
    $(".progressbar li").removeClass("active");
    $(".progressbar li").eq(0).addClass("active");
    $(".progressbar li").eq(1).addClass("active");
    
}
function Step3(action) {

    if ($("#table_kac tbody").html() == "" && action == "create") {
        GetStep3Data('create');
    }
    else if (action == "update") {
        GetStep3Data('update');
    }
    else if (action == "cdm") {
        GetStep3Data('cdm');
    }
    $(".step").addClass("d-none");
    $("#step3").removeClass("d-none");
    $(".progressbar li").removeClass("active");
    $(".progressbar li").eq(0).addClass("active");
    $(".progressbar li").eq(1).addClass("active");
    $(".progressbar li").eq(2).addClass("active");
}
function Step4() {
    $(".step").addClass("d-none");
    $("#step4").removeClass("d-none");
    $(".progressbar li").addClass("active");
    $("#step4Date").text(new Date().toString());
}
//script for grouping items
$(document).on('click', 'table .group-item', function () {
    $(this).parent().parent().toggleClass("table-info");
    $(this).children().toggleClass("fa-minus-circle");
    if ($(this).closest("table").find(".table-info").length == 0) {
        $(this).closest("section").find(".btn-success").attr("disabled", "disabled");
        var tablename = $(this).closest('table').attr('id');
        if (tablename == "table_branditems" || tablename == "table_cdmlaunches") {
            $("table .group-item").closest("section").find(".btn-success").prop("innerHTML", "No Items Selected");
        }
        else {
            $(this).closest("section").find(".groupedit").hide();
        }
    }
    else {
        $(this).closest("section").find(".btn-success").attr("disabled", false);
        var tablename = $(this).closest('table').attr('id');
        if (tablename == "table_branditems" || tablename == "table_cdmlaunches") {
            $("table .group-item").closest("section").find(".btn-success").prop("innerHTML", "Launch Selected Items");
        }
        else {
            $(this).closest("section").find(".groupedit").show();
            var count = $(this).closest("table").find(".table-info").length;
            $(this).closest("section").find(".groupeditcount").text(count);
        }
    }
});
$(document).on('click', 'table .btn-selectall', function () {
    var table = $(this).closest("table");
    table.DataTable().destroy();
    $(table).find("tbody tr").addClass("table-info");
    $(table).find(".fa-plus-circle").addClass("fa-minus-circle");
    var tablename = table.attr('id');
    if (tablename == "table_branditems" || tablename == "table_cdmlaunches") {
        $(this).closest("section").find(".btn-success").attr("disabled", false);
        $("table .group-item").closest("section").find(".btn-success").prop("innerHTML", "Launch Selected Items");
    }
    else {
        var count = $(this).closest("table").find(".table-info").length;
        $(this).closest("section").find(".groupeditcount").text(count);
        $(this).closest("section").find(".groupedit").show();
    }
    table.DataTable();
});
$(document).on('click', '.btn-groupedit', function () {
    $("#edit-modal").modal('show');
    var table = $(this).closest("section").find("table");
    var count = table.find(".table-info").length;
    $("#edit-modal .editmodal-count").text(count.toString());
    $("#edit-modal .ddl").html($("#select_sampleavail").html());
    $("#edit-modal .btn-save").unbind("click");
    $("#edit-modal .btn-save").on("click", function () {
        var sampleavail = $("#edit-modal .ddl").val();
        var commenttext = $("#edit-modal .txt-comment").val();
        table.DataTable().destroy();
        var rows = table.find(".table-info");
        var tablename = table.attr('id');
        if (tablename == "table_launchitems") {
            for (i = 0; i < count; i++) {
                rows.eq(i).find(".data-sampleavail").val(sampleavail);
                rows.eq(i).find(".data-comment").text(commenttext);
            }
        }
        table.DataTable();
        $("#edit-modal").modal('hide');
    })
});
$(document).on('click', '.btn-groupeditcdm1', function () {
    $("#groupedit-cdmdetails").modal('show');
    var table = $(this).closest("section").find("table");
    var count = table.find(".table-info").length;
    $("#groupedit-cdmdetails.editmodal-count").text(count.toString());
    $("#groupedit-cdmdetails .btn-save").unbind("click");
    $("#groupedit-cdmdetails .btn-save").on("click", function () {
        var category = $("#groupedit-cdmdetails .txt-category").val();
        var pogreview = $("#groupedit-cdmdetails .input-pogreview").val();
        var pogdrop = $("#groupedit-cdmdetails .input-pogdrop").val();
        var catcomment = $("#groupedit-cdmdetails .txt-catcomment").val();
        var competitive = $("#groupedit-cdmdetails .txt-competitive").val();
        table.DataTable().destroy();
        var rows = table.find(".table-info");
        for (i = 0; i < count; i++) {
            rows.eq(i).find(".data-category").text(category);
            rows.eq(i).find(".data-pogreview").text(pogreview);
            rows.eq(i).find(".data-pogdrop").text(pogdrop);
            rows.eq(i).find(".data-categorycomment").text(catcomment);
            rows.eq(i).find(".data-competitive").text(competitive);
            rows.eq(i).removeClass("table-info");
            rows.eq(i).find(".fa-plus-circle").removeClass("fa-minus-circle");
            $(this).closest("section").find(".groupedit").hide();
        }
        table.DataTable();
        $("#groupedit-cdmdetails").modal('hide');
    })
});
$(document).on('click', '.btn-groupeditcdm2', function () {
    $("#groupedit-status").val("");
    $("#groupedit-cdmdetails2").modal('show');
    var table = $(this).closest("section").find("table");
    var count = table.find(".table-info").length;
    $("#groupedit-cdmdetails2.editmodal-count").text(count.toString());
    $("#groupedit-cdmdetails2 .btn-save").unbind("click");
    $("#groupedit-cdmdetails2 .btn-save").on("click", function () {
        var status = parseInt($("#groupedit-status").val());
        var comment = $("#groupedit-cdmdetails2 .txt-comment").val();
        var followup = $("#groupedit-followupdate").val();
        table.DataTable().destroy();
        var rows = table.find(".table-info");
        for (i = 0; i < count; i++) {
            //Status
            if (rows.eq(i).find('.data-status option[value='+status+']').length != 0) {
                rows.eq(i).find('.data-status').val(status)
            }
            //Comment
            rows.eq(i).find(".data-comment").text(comment);
            //Follow up Date
            rows.eq(i).find(".data-followup").val(followup);
            //Reset Table
            rows.eq(i).removeClass("table-info");
            rows.eq(i).find(".fa-plus-circle").removeClass("fa-minus-circle");
            $(this).closest("section").find(".groupedit").hide();
            $("#groupedit-cdmdetails2 .txt-comment").val("");
            
        }
        table.DataTable();
        $("#groupedit-cdmdetails2").modal('hide');
    })
});
//Mass Update
//$("#table_cdmupdatelaunch").DataTable().rows('.table-info').data()
//$(this).closest('table').attr("id")

//Move data from step 1 to step 2
function GetStep2Data(src) {
    
    if (src == "create") {
        $('#table_launchitems').DataTable().clear();
        $('#table_launchitems').DataTable().destroy();
        var items = $("#table_branditems").DataTable().rows(".table-info").data();
        for (i = 0; i < items.length; i++) {
            var rowhtml = '<tr><td> <a href="#?" class="group-item"><i class="fa fa-plus-circle"></i></a></td><td>' + items[i][1] + '</td><td><span class="data-itemdesc">' + items[i][2] + '</span></td><td>' + items[i][3] + '</td>';
            rowhtml += '<td><div class="input-group"><div class="input-group-prepend"><span class="input-group-text"><i class="fa fa-cubes"></i></span></div><input type="text" class="form-control form-control-sm bg-white w1inv" disabled value="" /></div ><div class="input-group"><div class="input-group-prepend"><span class="input-group-text"><i class="fa fa-calendar"></i></span></div><input type="text" class="form-control form-control-sm bg-white w1date" disabled value="" /></div></td >';
            rowhtml += '<td><div class="input-group"><div class="input-group-prepend"><span class="input-group-text"><i class="fa fa-cubes"></i></span></div><input type="text" class="form-control form-control-sm bg-white w8inv" disabled value="" /></div ><div class="input-group"><div class="input-group-prepend"><span class="input-group-text"><i class="fa fa-calendar"></i></span></div><input type="text" class="form-control form-control-sm bg-white w8date" disabled value="" /></div></td >';
            rowhtml += '<td><div class="input-group"><div class="input-group-prepend"><span class="input-group-text"><i class="fa fa-cubes"></i></span></div><input type="text" class="form-control form-control-sm bg-white w9inv" disabled value="" /></div ><div class="input-group"><div class="input-group-prepend"><span class="input-group-text"><i class="fa fa-calendar"></i></span></div><input type="text" class="form-control form-control-sm bg-white w9date" disabled value="" /></div></td >';
            rowhtml += '<td><select class="form-control data-sampleavail">';
            rowhtml += $("#select_sampleavail").html();
            rowhtml += '</select></td>';
            rowhtml += '<td><button type="button" class="btn btn-outline-primary commentbtn">Comment</button></td><td hidden><label class="data-comment"></label></td></tr>';
            $("#table_launchitems tbody").append(rowhtml);
        }
        RetrieveInventory();
        $('#table_launchitems').DataTable({
            paging: false,
            searching: false,
            "columnDefs": [
                { "orderable": false, "targets": [0, 4, 5, 6, 7, 8, 9] }
            ],
            "order": [[1, "asc"]],
            initComplete: function () {
                $(this.api().table().container()).find('input').parent().wrap('<form>').parent().attr('autocomplete', 'off');
            }
        });
    }
    else if (src == "update") {
        $('#table_updateitems').DataTable().clear();
        
        var items = $("#table_bdmupdate").DataTable().rows(".table-info").data();
        for (i = 0; i < items.length; i++) {
            $('#table_updateitems').DataTable().destroy();
            var launchid = items[i][1];
            ListLaunchComments(items[i], launchid);
        }
    }
    else if (src == "cdm") {
        $('#table_cdmupdatelaunch').DataTable().clear();
        $('#table_cdmupdatelaunch').DataTable().destroy();
        var items = $("#table_cdmlaunches").DataTable().rows(".table-info").data();
        for (i = 0; i < items.length; i++) {
            var item = items[i];
            var rowhtml = '<tr><td> <a href="#?" class="group-item"><i class="fa fa-plus-circle"></i></a></td>';
            rowhtml += '<td hidden>' + item[1] + '</td >';
            rowhtml += '<td>' + item[2] + '</td >';
            rowhtml += '<td> <span class="data-itemdesc">' + item[3] + '</span></td >';
            rowhtml += '<td>' + item[4] + '</td >';
            //account
            rowhtml += '<td>' + item[5] + '</td>';
            rowhtml += '<td hidden>' + item[6] + '</td>';
            rowhtml += '<td>' + item[7] + '</td>';
            rowhtml += '<td hidden>' + item[8] + '</td>';
            
            rowhtml += '<td><div><button class="btn btn-outline-primary btn-block btn-cdmeditinfo">View / Edit Info</button></div></td>';
            rowhtml += '<td hidden class="data-category">' + item[12] + '</td>';
            rowhtml += '<td hidden class="data-pogreview">' + item[13].split(" ")[0] + '</td>';
            rowhtml += '<td hidden class="data-pogdrop">' + item[14].split(" ")[0] + '</td>';
            rowhtml += '<td hidden class="data-categorycomment">' + item[15] + '</td>';
            rowhtml += '<td hidden class="data-competitive">' + item[16] + '</td>';
            var currstatustext = $.parseHTML(item[11]);
            rowhtml += '<td hidden>' + currstatustext[1].textContent + '</td>';
            $("#table_cdmupdatelaunch tbody").append(rowhtml);
        }
        $('#table_cdmupdatelaunch').DataTable({
            paging: false,
            searching: false,
            "order": [[1, "asc"]]
        });
        $("#table_cdmupdatelaunch").removeAttr('style');
    }
    
}

$(document).on("click", ".commentbtn", function () {
    $("#comment-modal").modal('show');
    var datarow = $(this).parent().parent();
    var commentlabel = datarow.find(".data-comment");
    var itemdesc = datarow.find(".data-itemdesc");
    $("#commentmodal_itemdesc").text(itemdesc.text());
    $("#commentmodal_commentext").val(commentlabel.text());
    $("#commentmodal_savecomment").unbind("click");
    $("#commentmodal_savecomment").on("click", function () {
        
        commentlabel.text($("#commentmodal_commentext").val());
        
        $("#comment-modal").modal('hide');
    })
})

$(document).on("click", ".editcommentbtn", function () {
    $("#comment-modal .list-group").html("");
    $("#comment-modal").modal('show');
    var datarow = $(this).parent().parent();
    var commentlabel = datarow.find(".data-comment");
    var itemdesc = datarow.find(".data-itemdesc");
    $("#commentmodal_itemdesc").text(itemdesc.text());
    var commentlist = JSON.parse(datarow.find(".data-jsoncomment").text());
    for (i = 0; i < commentlist.length; i++) {
        var commenthtml= "";
        commenthtml += '<div class="list-group-item flex-column align-items-start">';
        commenthtml += '<div class="d-flex w-100 justify-content-between">';
        commenthtml += '<p class="mb-1">' + commentlist[i].CommentText + '</p><span><small>';
        commenthtml += commentlist[i].CommentDate + '</small></span></div ><small><b>' + commentlist[i].CommentUser + '</b></small></div >'
        $("#comment-modal .list-group").append(commenthtml);
    }
    $("#commentmodal_commentext").val(commentlabel.text());

    $("#commentmodal_savecomment").unbind("click");
    $("#commentmodal_savecomment").on("click", function () {
        $("#step2 .datatable").DataTable().destroy();
        commentlabel.text($("#commentmodal_commentext").val());
        $("#step2 .datatable").DataTable();
        $("#comment-modal").modal('hide');
    })
})
$(document).on("click", ".btn-cdmeditinfo", function () {
    $("#modal-cdmeditinfo").modal('show');
    var datarow = $(this).parent().parent().parent();
    var category = datarow.find(".data-category");
    var pogreview = datarow.find(".data-pogreview");
    var pogdrop = datarow.find(".data-pogdrop");
    var categorycomment = datarow.find(".data-categorycomment");
    var competitive = datarow.find(".data-competitive");
    var itemdesc = datarow.find(".data-itemdesc");
    $("#commentmodal_itemdesc").text(itemdesc.text());
    $("#modal-cdmeditinfo .itemdesc").text(itemdesc.text());
    $("#modal-cdmeditinfo .category").val(category.text());
    $("#modal-cdmeditinfo .pogreview").val(pogreview.text());
    $("#modal-cdmeditinfo .pogdrop").val(pogdrop.text());
    $("#modal-cdmeditinfo .categorycomment").val(categorycomment.text());
    $("#modal-cdmeditinfo .competitive").val(competitive.text());

    $("#btn-updatecdminfo").unbind("click");
    $("#btn-updatecdminfo").on("click", function () {
        $("#table_cdmupdatelaunch").DataTable().destroy();
        category.text($("#modal-cdmeditinfo .category").val());
        pogreview.text($("#modal-cdmeditinfo .pogreview").val());
        pogdrop.text($("#modal-cdmeditinfo .pogdrop").val());
        categorycomment.text($("#modal-cdmeditinfo .categorycomment").val());
        competitive.text($("#modal-cdmeditinfo .competitive").val());
        $("#step2 .datatable").DataTable();
        $("#modal-cdmeditinfo").modal('hide');
    })

})
$(document).on("click", ".editcommentbtn-cdm", function () {
    $("#comment-modal .list-group").html("");
    $("#comment-modal").modal('show');
    var datarow = $(this).parent().parent();
    var commentlabel = datarow.find(".data-comment");
    var itemdesc = datarow.find(".data-itemdesc");
    $("#commentmodal_itemdesc").text(itemdesc.text());
    if (datarow.find(".data-jsoncomment").text() != "") {
        var commentlist = JSON.parse(datarow.find(".data-jsoncomment").text());
        for (i = 0; i < commentlist.length; i++) {
            var commenthtml = "";
            commenthtml += '<div class="list-group-item flex-column align-items-start">';
            commenthtml += '<div class="d-flex w-100 justify-content-between">';
            commenthtml += '<p class="mb-1">' + commentlist[i].CommentText + '</p><span><small>';
            commenthtml += commentlist[i].CommentDate + '</small></span></div ><small><b>' + commentlist[i].CommentUser + '</b></small></div >'
            $("#comment-modal .list-group").append(commenthtml);
        }
    }
    
    $("#commentmodal_commentext").val(commentlabel.text());

    $("#commentmodal_savecomment").unbind("click");
    $("#commentmodal_savecomment").on("click", function () {
        $("#step3 .datatable").DataTable().destroy();
        commentlabel.text($("#commentmodal_commentext").val());
        $("#step3 .datatable").DataTable();
        $("#comment-modal").modal('hide');
    })
})
//Run script when going to page 3
function GetStep3Data(src) {
    if (src == "create") {
        $('#table_kac').DataTable().clear();
        $('#table_kac').DataTable().destroy();
        GetKeyAccounts();
    }
    else if (src == "update") {
        var items = $("#table_updateitems").DataTable().rows().data();
        $('#table_updatestatus').DataTable().clear();
        $('#table_updatestatus').DataTable().destroy();

        for (i = 0; i < items.length; i++) {
            var rowhtml = '<tr>';
            var launchid = $.parseHTML(items[i][1]);
            rowhtml += '<td hidden><span class="data-launchid">' + launchid[0].textContent + '</span></td>';//launchid
            rowhtml += '<td>' + items[i][2] + '</td>';//itemnum
            var itemdesc = $.parseHTML(items[i][3]);
            rowhtml += '<td>' + itemdesc[0].textContent + '</td>';//item desc
            rowhtml += '<td>' + items[i][4] + '</td>';//unit size

            var kac = $.parseHTML(items[i][5]);
            rowhtml += '<td>' + kac[0].textContent + '</td>';//account
            if (items[i][6] == null || items[i][6] == "") {//banner
                rowhtml += '<td></td>';
            }
            else {
                rowhtml += '<td>' + items[i][6] + '</td>';
            }
            rowhtml += '<td hidden><span class="data-launchtype">' + items[i][7] + '</span></td>';//launchtype
            rowhtml += '<td hidden><span class="data-sampleavail">' + items[i][8] + '</span></td>';//sampleavail
            var comment = $.parseHTML(items[i][11]);
            rowhtml += '<td hidden><span class="data-comment">' + comment[0].textContent + '</span></td>';//comment
            rowhtml += '<td><select class="form-control">';
            rowhtml += $("#select_launchstatus").html();
            rowhtml += '</select></td>'
            $("#table_updatestatus tbody").append(rowhtml);
        }
    }
    else if (src == "cdm") {
        $('#table_cdmstatuscomment').DataTable().clear();
        
        var items = $("#table_cdmupdatelaunch").DataTable().rows().data();
        for (i = 0; i < items.length; i++) {
            
            var launchid = items[i][1];
            ListComments(items[i], launchid);
            $("#table_cdmstatuscomment tr").removeAttr('style');
        }
        
    }
    
}
function CDMDynamicStatus(currstatus) {
    var html = "";
    var statuslist = $.parseHTML($("#select_status").html());
    switch (currstatus) {
        case "Ready to Present":
            
            html += statuslist[3].outerHTML;
            html += statuslist[4].outerHTML;
            html += statuslist[5].outerHTML;
            html += statuslist[6].outerHTML;
            html += statuslist[7].outerHTML;
            html += statuslist[8].outerHTML;
            html += statuslist[9].outerHTML;
            html += statuslist[10].outerHTML;
            html += statuslist[11].outerHTML;
            html += statuslist[12].outerHTML;
            html += statuslist[0].outerHTML;
            break;
        case "VOR Change Initiated":
            html += statuslist[4].outerHTML;
            html += statuslist[5].outerHTML;
            break;
        case "Initial Contact":
            html += statuslist[6].outerHTML;
            html += statuslist[7].outerHTML;
            html += statuslist[8].outerHTML;
            html += statuslist[9].outerHTML;
            html += statuslist[10].outerHTML;
            html += statuslist[11].outerHTML;
            html += statuslist[12].outerHTML;
            break;
        case "Initial Meeting":
            html += statuslist[6].outerHTML;
            html += statuslist[7].outerHTML;
            html += statuslist[8].outerHTML;
            html += statuslist[9].outerHTML;
            html += statuslist[10].outerHTML;
            html += statuslist[11].outerHTML;
            html += statuslist[12].outerHTML;
            break;
        case "Launch Package Sent":
            html += statuslist[6].outerHTML;
            html += statuslist[7].outerHTML;
            html += statuslist[8].outerHTML;
            html += statuslist[9].outerHTML;
            html += statuslist[10].outerHTML;
            html += statuslist[11].outerHTML;
            html += statuslist[12].outerHTML;
            break;
        case "Pending":
            html += statuslist[9].outerHTML;
            html += statuslist[10].outerHTML;
            html += statuslist[11].outerHTML;
            html += statuslist[12].outerHTML;
            break;
        default:
            html += $("#select_status").html();
            break;

    }
    return html;
}
function scrolltotop() {
    $(window).scrollTop(0);
}
function togglepnltable() {
    $("#pnltable").slideToggle();
}
function warningSave() {
    $("#modalWarningSave").modal('toggle');
}
function warningDelete() {
    $("#modalDeleteGroup").modal('toggle');
    $("#modalWarningSave").modal('toggle');
}

