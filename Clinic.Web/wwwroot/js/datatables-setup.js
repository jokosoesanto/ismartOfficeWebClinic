/**
 * Enterprise DataTables Foundation v2.0
 * 
 * SINGLE initialization pattern for the entire Enterprise.
 * All tables MUST use class="enterprise-grid" and configure via data-* attributes.
 * Manual $.DataTable() calls in Views are FORBIDDEN.
 * 
 * =====================================================================
 * TIER 1: Simple Tables (Doctor, Patient, NumberSequence, Admin, Specialty)
 * =====================================================================
 * Use class="enterprise-grid" with data-* attributes.
 * No JavaScript required in the View.
 * 
 * Supported data attributes:
 *   data-page-length        (int)     Default: 10
 *   data-order              (json)    Default: [] (no initial sort)
 *   data-search-placeholder (string)  Default: "Filter records:"
 *   data-responsive         (bool)    Default: true
 *   data-scrollx            (bool)    Default: false
 *   data-state-save         (bool)    Default: true
 *   data-auto-width         (bool)    Default: true
 *   data-dom                (string)  Default: Enterprise standard DOM
 * 
 * =====================================================================
 * TIER 2: Advanced Tables (MasterReference, ScheduleBoard, Appointment)
 * =====================================================================
 * Tables requiring server-side processing, custom column renderers,
 * or dynamic AJAX data callbacks MUST register their configuration
 * via window.EnterpriseDataTables.register() BEFORE $(document).ready().
 * 
 * The table element still uses class="enterprise-grid" for consistent
 * styling but delegates full config to the registered definition.
 * 
 * Usage:
 *   window.EnterpriseDataTables.register('tableId', { ...DataTable config });
 * 
 * The Foundation will merge Enterprise defaults with the registered config.
 */
(function () {
    'use strict';

    // Registry for advanced table configurations
    var _registry = {};

    window.EnterpriseDataTables = {
        /**
         * Register an advanced DataTable configuration.
         * Must be called BEFORE $(document).ready().
         * @param {string} tableId - The DOM id of the table (without #).
         * @param {object} config  - Full DataTable configuration object.
         */
        register: function (tableId, config) {
            _registry[tableId] = config;
        }
    };

    $(document).ready(function () {
        if (!$.fn.DataTable) return;

        var ENTERPRISE_DOM =
            "<'row mb-3'<'col-md-6'l><'col-md-6 text-end'f>>" +
            "<'row'<'col-sm-12'tr>>" +
            "<'row mt-3'<'col-md-5'i><'col-md-7'p>>";

        $('.enterprise-grid').each(function () {
            var $table = $(this);
            var tableId = $table.attr('id');

            // Guard: never reinitialise
            if ($.fn.DataTable.isDataTable(this)) return;

            var config;

            if (tableId && _registry[tableId]) {
                // TIER 2: Advanced — use registered config with Enterprise defaults
                var registered = _registry[tableId];
                config = $.extend(true, {
                    pageLength: 10,
                    responsive: true,
                    stateSave: true,
                    autoWidth: true,
                    dom: ENTERPRISE_DOM,
                    language: {
                        search: '',
                        searchPlaceholder: 'Filter records:',
                        processing: 'Processing...',
                        emptyTable: 'No data available in table'
                    }
                }, registered);
            } else {
                // TIER 1: Simple — read configuration from data attributes
                var pageLength       = parseInt($table.data('page-length')) || 10;
                var orderAttr        = $table.data('order');
                var order            = (orderAttr !== undefined && orderAttr !== null) ? orderAttr : [];
                var searchPlaceholder = $table.data('search-placeholder') || 'Filter records:';
                var responsive       = $table.data('responsive') !== false;
                var scrollX          = $table.data('scrollx') === true;
                var stateSave        = $table.data('state-save') !== false;
                var autoWidth        = $table.data('auto-width') !== false;
                var dom              = $table.data('dom') || ENTERPRISE_DOM;

                config = {
                    pageLength: pageLength,
                    order: order,
                    responsive: responsive,
                    scrollX: scrollX,
                    stateSave: stateSave,
                    autoWidth: autoWidth,
                    dom: dom,
                    language: {
                        search: '',
                        searchPlaceholder: searchPlaceholder,
                        processing: 'Processing...',
                        emptyTable: 'No data available in table'
                    }
                };
            }

            // Initialize
            $table.DataTable(config);

            // Enterprise styling for Bootstrap integration
            var $wrapper = $table.closest('.dataTables_wrapper');
            $wrapper.find('.dataTables_filter input').addClass('form-control shadow-sm border-0 bg-light');
            $wrapper.find('.dataTables_length select').addClass('form-select shadow-sm border-0 bg-light');
        });
    });
})();
