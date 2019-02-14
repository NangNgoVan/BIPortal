
; (function ($, window, document, undefined) {
    var pluginName = "editable",
        defaults = {
            keyboard: true,
            dblclick: false,
            button: true,
            buttonSelector: ".edit",
            buttonCanceler: ".cancel",
            buttonCreate: ".create",
            maintainWidth: true,
            dropdowns: {},
            edit: function () { },
            save: function () { },
            cancel: function () { },
            create: function () { },
            validate: function () { return true; }
        };

    function editable(element, options) {
        this.element = element;
        this.options = $.extend({}, defaults, options);

        this._defaults = defaults;
        this._name = pluginName;

        this.init();
    }

    editable.prototype = {
        init: function () {
            this.editing = false;

            if (this.options.dblclick) {
                $(this.element)
                    .css('cursor', 'pointer')
                    .bind('dblclick', this.toggle.bind(this));
            }

            if (this.options.button) {
                $(this.options.buttonSelector, this.element)
                    .bind('click', this.toggle.bind(this));

                $(this.options.buttonCanceler, this.element)
                    .bind('click', this.delete.bind(this));
                $(this.options.buttonCreate, this.element)
                    .bind('click', this.create.bind(this));
            }
        },

        toggle: function (e) {
            e.preventDefault();

            this.editing = !this.editing;

            if (this.editing) {
                this.edit();
            } else {
                this.save();
            }
        },

        delete: function (e) {
            e.preventDefault();
            this.editing = !this.editing;

            if (this.editing) {
                this.edit();
            } else {
                this.cancel();
            }
        }
        ,

        edit: function () {
            var instance = this,
                values = {};

            $('td[data-field]', this.element).each(function () {
                var input,
                    field = $(this).data('field'),
                    readonly = $(this).is('[data-readonly]') ? $(this).data('readonly') : false,
                    value = $(this).text(),
                    width = $(this).width();

                values[field] = value;

                $(this).empty();

                if (instance.options.maintainWidth) {
                    $(this).width(width);
                }

                if (field in instance.options.dropdowns) {
                    if (readonly)
                        input = $('<select disabled></select>');
                    else
                        input = $('<select required></select>');

                    for (var i = 0; i < instance.options.dropdowns[field].length; i++) {
                        $('<option required value="' + instance.options.dropdowns[field][i].Value + '"></option>')
                            .text(instance.options.dropdowns[field][i].Name)
                            .appendTo(input);
                    };

                    input.val($(this).data('value'))
                        .data('old-value', value)
                        .dblclick(instance._captureEvent);
                } else {
                    if (readonly)
                        input = $('<input type="text" disabled/>')
                            .val(value)
                            .data('old-value', value)
                            .dblclick(instance._captureEvent);
                    else {
                        input = $('<input type="text"/>')
                            .val(value)
                            .data('old-value', value)
                            .dblclick(instance._captureEvent);
                    }
                }

                input.appendTo(this);

                if (instance.options.keyboard) {
                    input.keydown(instance._captureKey.bind(instance));
                }
            });

            this.options.edit.bind(this.element)(values);
        },

        save: function () {
            var instance = this,
                values = {};
            $('td[data-field]', this.element).each(function () {
                var value = $(':input', this).val();

                values[$(this).data('field')] = value;

            });
            if (this.options.validate.bind(this.element)(values)) {
                $('td[data-field]', this.element).each(function () {
                    var value = $(':input', this).val();
                    var text = $(':input :selected', this).text();
                    var field = $(this).data('field');
                    if (field in instance.options.dropdowns) {
                        $(this).empty()
                            .text(text);
                        $(this).data('value', value);
                    }
                    else
                        $(this).empty()
                            .text(value);
                });
                this.options.save.bind(this.element)(values);
            }
            else
                this.editing = true;
        },

        cancel: function () {
            var instance = this,
                values = {};

            $('td[data-field]', this.element).each(function () {
                var value = $(':input', this).data('old-value');

                values[$(this).data('field')] = value;

                $(this).empty()
                    .text(value);
            });

            this.options.cancel.bind(this.element)(values);
        },
        create: function () {
            var instance = this,
                values = {};
            if (this.editing) {
                $('td[data-field]', this.element).each(function () {
                    var value = $(':input', this).val();

                    values[$(this).data('field')] = value;

                });
            }
            else {
                $('td[data-field]', this.element).each(function () {

                    values[$(this).data('field')] = $(this).text();

                });
            }
            this.options.create.bind(this.element)(values);
        },

        _captureEvent: function (e) {
            e.stopPropagation();
        },

        _captureKey: function (e) {
            if (e.which === 13) {
                this.editing = false;
                this.save();
            } else if (e.which === 27) {
                this.editing = false;
                this.cancel();
            }
        }
    };

    $.fn[pluginName] = function (options) {
        return this.each(function () {
            if (!$.data(this, "plugin_" + pluginName)) {
                $.data(this, "plugin_" + pluginName,
                    new editable(this, options));
            }
        });
    };

})(jQuery, window, document);
