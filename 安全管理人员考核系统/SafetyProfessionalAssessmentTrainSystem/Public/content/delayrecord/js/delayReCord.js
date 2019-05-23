
$(function () {
    var controllerName = "DelayReCord";
    var $gridCertificate = $("#gridDelayReCord_main");
    var $pagerCertificate = $("#pagerDelayReCord_main");
    var $pageContainer = $('#divDelayReCord_PageContainer');
    var $divQueryArea = $pageContainer.find("[name='divQueryArea']");




    var getSelectedRowDataOfGrid = function (rowid) {
        var selRowId = "";
        if (!rowid) {
            $gridCertificate.jqGrid("getGridParam", "selrow");
        }
        selRowId = rowid;
        var rowData = {};
        if (selRowId && null != selRowId) {
            rowData = $gridCertificate.jqGrid("getRowData", selRowId);
        }
        else {
            abortThread("请选中行！");
        }
        return rowData;
    }


    var initCertificateGrid = function () {
        var queryData = {};
        queryData = getJson($divQueryArea);
        $gridCertificate.jqGrid({
            url: "/" + controllerName + "/GetCertificateListForJqgrid",
            datatype: "json",
            multiselect: true,
            multiboxonly: true,
            postData: queryData,
            colNames: ["Id", "certificateId", "姓名", "性别", "年龄", "身份证号", "职务", "技术职称", "行业", "证书类型", "证书编号", "发证日期", "有效期", "培训机构", "当前状态", "状态修改日期"],
            colModel: [
                    { name: "employeeId", index: "employeeId", width: 30, hidden: true },
                    { name: "certificateId", index: "certificateId", width: 30, hidden: true },
                    { name: "employeeName", index: "employeeName", align: "center", width: 80 },
                    { name: "sex", index: "sex", align: "center", width: 50 },
                    { name: "age", index: "age", align: "center", width: 50 },
                    { name: "iDNumber", index: "iDNumber", align: "center", width: 170 },
                    { name: "job", index: "Job", align: "center", width: 80 },
                    { name: "title4Technical", index: "title4Technical", align: "center", width: 80 },
                    { name: "industry", index: "industry", align: "center", width: 80 },
                    { name: "examType", index: "examType", align: "center", width: 80 },
                    { name: "certificateNo", index: "certificateNo", align: "center", width: 180 },
                    { name: "startCertificateDate", index: "startCertificateDate", align: "center", width: 80 },
                    { name: "endCertificateDate", index: "endCertificateDate", align: "center", width: 80 },
                    { name: "trainingInstitutionName", index: "trainingInstitutionName", align: "center", width: 150 },
                    { name: "currentStatus", index: "submitStatus", align: "center", width: 120 },
                    { name: "operationDate", index: "operationDate", align: "center", width: 120 }
            ],
            autowidth: true,
            rowNum: 20,
            altRows: true,
            pgbuttons: true,
            viewrecords: true,
            shrinkToFit: true,
            pginput: true,
            rowList: [10, 20, 30, 50, 70, 100],
            pager: $pagerCertificate,
            ondblClickRow: function (rowid, iRow, iCol, e) {
                $gridCertificate.jqGrid("toggleSubGridRow", rowid);
            },
            subGrid: true,
            subGridOptions: {
                "plusicon": "ace-icon fa fa-plus",
                "minusicon": "ace-icon fa fa-minus",
                "openicon": "ace-icon fa fa-share",
            },
            subGridRowExpanded: function (subgrid_id, row_id) {
                var subgrid_table_id, pager_id;
                var rowData = $gridCertificate.jqGrid("getRowData", row_id);
                subgrid_table_id = subgrid_id + "_t";
                pager_id = "p_" + subgrid_table_id;
                $("#" + subgrid_id).html("<div style='width:100%;overflow:auto'><table id='" + subgrid_table_id + "' class='scroll' ></table></div>");
                var subGridQueryData = {};
                subGridQueryData.certificateId = rowData.certificateId;
                $("#" + subgrid_table_id).jqGrid({
                    url: "/" + controllerName + "/GetWorkFlow",
                    datatype: "json",
                    postData: subGridQueryData,
                    rownumbers: true,
                    rowNum: 10,
                    colNames: ["操作时间", "操作人", "操作", "备注"],
                    colModel: [
                        { name: "OperaDateTime", index: "OperaDateTime", width: "150", align: "center", },
                        { name: "OperaUserName", index: "OperaUserName", width: "300", align: "center", },
                        { name: "Operation", index: "Operation", width: "150", align: "center", },
                        { name: "Remark", index: "Remark", width: "300", align: "center", }

                    ],
                    autoWidth: false,
                    ondblClickRow: function (rowid, iRow, iCol, e) {
                        return false;
                    },
                    loadComplete: function () {

                    }
                });
            },
            loadComplete: function () {
                var table = this;
                updatePagerIcons(table);
                jqGridAutoWidth();
                setGridHeight($gridCertificate.selector);
            }
        });
    }

    var initQueryArea = function () {

        var initQueryButton = function () { 
            $pageContainer.find("[name='btnQuery']").on("click", function () {
                var queryData = {};
                queryData = getJson($divQueryArea);
                $gridCertificate.jqGrid("setGridParam", { page: 1, postData: queryData }).trigger("reloadGrid");
            })
        }

        initQueryButton();
    }


    $(document).ready(function () {
        initQueryArea();
        initCertificateGrid();

    })
})
