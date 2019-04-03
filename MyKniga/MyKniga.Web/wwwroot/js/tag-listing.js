function refreshTags() {
    $.ajax({
        url: '/tags/gettags',
        type: 'get',
        data: {
            searchQuery: $('#searchQuery').val()
        }
    }).done(function (response) {
        let tagHtml = '';
        if (response.tags.length > 0) {
            response.tags.forEach(function (tag) {
                tagHtml +=
                    `<span class="badge badge-pill bg-primary rounded-corners p-2 text-white" data-tagid="${escapeHtml(tag.id)}">
                        ${escapeHtml(tag.name)}
                        <button type="button" class="btn btn-link p-0 m-0 text-light font-weight-bold" onclick="deleteTag(this)">
                            <i class="fas fa-times-circle"></i>
                        </button>
                    </span>`;
            });
        } else {
            tagHtml = `<h3 class="col-12 text-center text-muted">No tags found</h3>`;
        }

        $('#tagsList').html(tagHtml);
    });
}

function deleteTag(btn) {
    let parent = $(btn).parent();

    let tagId = parent.data('tagid');

    $.ajax({
        url: '/tags/deletetag',
        type: 'post',
        data: {
            tagId: tagId,
            __RequestVerificationToken:
                $('#verificationForm input[name=__RequestVerificationToken]').val()
        },
        success: function (result) {
            if (!result.success) {
                return;
            }

            parent.remove();
        }
    });
}

function createTag() {
    let field = $('#addTagField');

    let tagName = field.val();

    if (tagName == null || tagName.length === 0) {
        return;
    }

    $.ajax({
        url: '/tags/create',
        type: 'POST',
        data: {
            name: tagName,
            __RequestVerificationToken:
                $('#verificationForm input[name=__RequestVerificationToken]').val()
        },
        success: function (result) {
            if (!result.success) {
                return;
            }

            field.val('');
            refreshTags();
        }
    });
}


(function () {
    let searchQuery = $('#searchQuery');
    searchQuery.keypress(refreshTags);
    searchQuery.change(refreshTags);

    refreshTags();
})();