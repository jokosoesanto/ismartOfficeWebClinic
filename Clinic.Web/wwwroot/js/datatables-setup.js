$(document).ready(function () {
    if ($.fn.DataTable) {
        $('.enterprise-grid').each(function () {
            if (!$.fn.DataTable.isDataTable(this)) {
                $(this).DataTable({
                    autoWidth: true,
                    responsive: true,
                    stateSave: true,
                    pageLength: 10,
                    language: {
                        search: "Filter records:",
                        processing: "Processing...",
                        emptyTable: "No data available in table"
                    }
                });
            }
        });
    }
});
