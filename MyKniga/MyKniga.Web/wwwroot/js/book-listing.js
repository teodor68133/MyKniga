function refreshBooks() {
    $.ajax({
        url: '/books/getbooks',
        type: 'get'
    }).done(function (response) {
        let bookHtml = '';
        response.forEach(function (book) {
            bookHtml +=
                `<div class="col-6 col-md-4 col-lg-3">
                    <div class="card m-1 shadow">
                        <img src="${escapeHtml(book.imageUrl)}" class="card-img-top book-thumbnail-img" 
                             alt="${escapeHtml(book.title)}">
                        <div class="card-body">
                            <h6 class="card-title">${escapeHtml(book.title)}</h6>
                            <p class="card-text">€${book.price}
                                <br>
                                ${escapeHtml(book.author)}
                            </p>
                            <a href="/books/details/${book.id}" class="btn btn-link stretched-link">Details</a>
                        </div>
                    </div>
                </div>`;
        });

        $('#booksList').html(bookHtml);
    });
}

(function () {
    refreshBooks();
})();