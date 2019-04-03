function refreshBooks() {
    $.ajax({
        url: '/books/getbooks',
        type: 'get',
        data: $('#filterForm').serialize()
    }).done(function (response) {
        let bookHtml = '';
        if (response.length > 0) {
            response.forEach(function (book) {
                bookHtml +=
                    `<div class="col-6 col-md-4 p-1">
                    <div class="card h-100">
                        <img src="${escapeHtml(book.imageUrl)}" class="card-img-top book-thumbnail-img" 
                             alt="${escapeHtml(book.title)}">
                        <div class="card-body">
                            <h6 class="card-title">
                                <a href="/books/details/${book.id}" class="text-dark stretched-link">
                                    ${escapeHtml(book.title)}
                                </a>
                            </h6>
                            <p class="card-text">€${book.price}
                                <br>
                                ${escapeHtml(book.author)}
                            </p>
                        </div>
                    </div>
                </div>`;
            });
        } else {
            bookHtml = `<h3 class="col-12 text-center text-muted">No books found</h3>`;
        }

        $('#booksList').html(bookHtml);
    });
}

(function () {
    refreshBooks();
})();