// actions after document is ready
$(document).ready(function () {
    // Ensure checkbox work in Ajax calls
    $('input[type=checkbox]').on('change', function () {
        var name = $(this).attr('name');
        if ($(this).is(':checked')) {
            $('input[name= "' + name + '"]').val(true);
            $(this).val(true);
        }
        else {
            $(this).val(false);
            $('input[name= "' + name + '"]').val(false);
        }
    });

    // Change dropdown to hover
    $(".dropdown").hover(
        function () {
            $('.dropdown-menu', this).stop(true, true).fadeIn("fast");
            $(this).toggleClass('open');
        },
        function () {
            $('.dropdown-menu', this).stop(true, true).fadeOut("fast");
            $(this).toggleClass('open');
        });

    // Enable Tooltips
    $('[data-toggle="tooltip"]').tooltip();

    // Modals
    $(".alwaysShowModal").each(function () {
        var modalWindow = $(this);
        var linkSelector = $(this).data('link');
        var link = $(linkSelector).attr('href');
        modalWindow.modal("show");
        setTimeout(function () {
            window.location.replace(link);
        }, 5000);
    });

    // Mega menu
    $('li.mega-menu').each(function (idx, val) {
        var setCount = $(this).data('set');
        var set = setCount, //Number of links to display in each column
            buffer = [],
            dropdown = $('.dropdown-menu', this),
            children = dropdown.children(),
            cols = Math.ceil(children.length / set),
            col_class = 'col-6 col-md-' + (cols >= 5 ? '2' : (cols === 4 ? '3' : (cols === 3 ? '4' : 'x'))),
            container_class = 'px-0 container container-' + (cols === 2 ? 'sm' : (cols === 3 ? 'md' : (cols === 4 ? 'lg' : (cols >= 5 ? 'xl' : 'x'))));

        for (var i = 0; i < cols; i++) {
            buffer.push('<div class="' + col_class + '">');
            children.slice(i * set, (i + 1) * set).each(function () {
                buffer.push($(this).prop('outerHTML'));
            });
            buffer.push('</div>');
        }

        dropdown.html('<div class="' + container_class + '"><div class="row">' + buffer.join('\n') + '</div></div>');
    });
    // page of content
    tableOfContents("#tableOfContent");
    // Sticky polyfill
    $('.sticky').Stickyfill();

    // https://stackoverflow.com/questions/42086841/jquery-validate-with-summernote-editor-error-cannot-read-property-replace-of
    // Add comment click, rich text editor handlers.
    $('.text-editor-form').each(function () {
        var editorArea = $(this);
        var formSelector = $(this).data('form-id');
        var f = $(formSelector);
        var textAreaSelector = $(this).data('te-name')
        var textAreaElement = $(textAreaSelector);
        var submitBtn = $($(this).data('submit-btn'));
        // data-succeed-modal="#modelAddCommentSucceed" data-fail-modal="#modelAddCommentFailed" data-fail-msg-name="#failedMsg"
        var modalSucceedSelector = $(this).data('succeed-modal');
        var modalFailSelector = $(this).data('fail-modal');
        var modalFailMsgSelector = $(this).data('fail-msg-name');
        var modalInProgressSelector = $(this).data('progress-modal');

        var v = f.validate();

        if (f.data('validator'))
            f.data('validator').settings.ignore = ":hidden:not(" + textAreaSelector + "),.note-editable.card-block";


        // on focus
        textAreaElement.focus(function () {
            loadEditor($(this), v);
        });

        // on submit 
        if (submitBtn) {
            submitBtn.click(function () {
                f.validate();
                if (f.valid()) {
                    submitBtn.attr("disabled", "disabled");
                    // post
                    var action = f.attr('action');
                    var requestToken = $("#requestToken").val();
                    var requestTokenName = $("#requestTokenName").val();
                    var headers = {};
                    headers[requestTokenName] = requestToken;
                    headers['Content-Type'] = 'application/json; charset=utf-8';
                    $(modalInProgressSelector).modal("show");
                    $.ajax(
                        {
                            url: action,
                            data: JSON.stringify(convertFormToJSON(f)),
                            method: "POST",
                            headers: headers,
                            success: function (data, textStatus, jqXHR) {
                                var result = data;
                                setTimeout(function () {
                                    $(modalInProgressSelector).modal('hide');
                                    if (result.isSuccessful) {
                                        $(modalSucceedSelector).on('hidden.bs.modal', function (e) {
                                            refreshPage();
                                        })
                                        $(modalSucceedSelector).modal("show");
                                        submitBtn.removeAttr("disabled");
                                    }
                                    else {
                                        $(modalFailMsgSelector).text(result.errorMessage);
                                        $(modalFailSelector).modal("show");
                                        submitBtn.removeAttr("disabled");
                                    }
                                }, 1000);
                            },
                            error: function (jqXHR, textStatus, errorThrown) {
                                setTimeout(function () {
                                    $(modalInProgressSelector).modal('hide');
                                    $(modalFailMsgSelector).text(textStatus);
                                    $(modalFailSelector).modal("show");
                                    submitBtn.removeAttr("disabled");
                                }, 1000);
                            }
                        });

                }
            });
        }
    });

    // Rich text editor
    $('.rich-text-editor').each(function () {
        var formSelector = $(this).data('form-id');
        var f = $(formSelector);
        var v = f.validate({
            ignore: ":hidden:not(" + textAreaSelector + "),.note-editable.card-block"
        });
        var textAreaSelector = $(this).data('te-name')
        var textAreaElement = $(textAreaSelector);
        loadEditor(textAreaElement, v);
    });

    // View count update for blog post page.

    updateBlogPostViewCount('#viewCount');

});

function refreshCaptcha(imageId, url) {
    var randomStr = Math.random().toString(36).substring(7);
    $(imageId).attr('src', url + '?' + randomStr);
}

function updateBlogPostViewCount(viewCountSelector) {
    var viewCountElement = $(viewCountSelector);
    if (viewCountElement) {
        var tnvSelector = viewCountElement.data('tnv-selector');
        var tnSelector = viewCountElement.data('tn-selector');
        var url = viewCountElement.data('update-url');

        var headers = {};
        headers[$(tnSelector).val()] = $(tnvSelector).val();
        $.ajax(
            {
                url: url,
                method: "GET",
                success: function (data, textStatus, jqXHR) {
                    viewCountElement.text(data.viewCount);
                },
                headers: headers
            }
        );
    }
}

/**
 * Refresh page
 */
function refreshPage() {
    location.reload();
}

/**
 * Convert form to JSON
 * @param {any} form
 */
function convertFormToJSON(form) {
    var array = form.serializeArray();
    var json = {};

    $.each(array, function () {
        json[this.name] = this.value || '';
    });

    return json;
}

/**
 * Display all the headings in the page
 * @param {any} selector
 */
function tableOfContents(selector) {
    var el = $(selector);
    var targetSelector = el.data("target");
    var containerSelector = el.data("toc");
    var headingsSelector = el.data("headings");
    $(containerSelector).toc({ content: targetSelector, headings: headingsSelector });
    $('body').scrollspy({ target: containerSelector });
}


/**
 * Reply to blog comment
 * @param {any} blogPostCommentId
 * @param {any} replyToAuthor
 * @param {any} selectorForReplyToComment
 * @param {any} selectorForCommentArea
 */
function replyToBlogComment(blogPostCommentId, replyToAuthor, selectorForReplyToComment, selectorForCommentArea) {
    $(selectorForReplyToComment).prop('value', blogPostCommentId);
    var commentAreaEl = $(selectorForCommentArea);
    commentAreaEl.text("@" + replyToAuthor);
    commentAreaEl.focus();
    if (commentAreaEl.validate)
        commentAreaEl.validate();
}

/**
 * Load editor (summer note)
 * @param {any} inputElement
 * @param {any} validator
 */
function loadEditor(inputElement, validator) {
    inputElement.summernote({
        toolbar: [
            // [groupName, [list of button]]
            ['style', ['bold', 'italic', 'underline', 'clear']],
            ['font', ['strikethrough', 'superscript', 'subscript']],
            ['fontsize', ['fontsize']],
            ['color', ['color']],
            ['para', ['ul', 'ol', 'paragraph']],
            ['insert', ['picture', 'link', 'table', 'hr']],
            ['height', ['height']]
        ],
        minHeight: null,             // set minimum height of editor
        maxHeight: null,             // set maximum height of editor
        focus: true,
        callbacks: {
            onChange: function (contents, $editable) {
                // Note that at this point, the value of the `textarea` is not the same as the one
                // you entered into the summernote editor, so you have to set it yourself to make
                // the validation consistent and in sync with the value.
                inputElement.val(inputElement.summernote('isEmpty') ? "" : contents);

                // You should re-validate your element after change, because the plugin will have
                // no way to know that the value of your `textarea` has been changed if the change
                // was done programmatically.
                validator.element(inputElement);
            }
        }
    });
}

