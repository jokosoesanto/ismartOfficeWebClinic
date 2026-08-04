$(document).ready(function () {
    if ($.fn.DataTable) {
        // Find if there is a table with enterprise-grid class
        var table = $('.enterprise-grid').DataTable({
            autoWidth: true,
            responsive: true,
            stateSave: true,
            pageLength: 10,
            language: {
                search: "Filter records:",
                processing: "Processing..."
            }
        });
    }
});
