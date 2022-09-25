function dragStart(e) {
    this.style.opacity = '0.5';
    dragSrcEl = this;
    e.dataTransfer.effectAllowed = 'move';
    e.dataTransfer.setData('text/html', this.innerHTML);
};

function dragEnter(e) {
    this.classList.add('over');
}

function dragLeave(e) {
    e.stopPropagation();
    this.classList.remove('over');
}

function dragOver(e) {
    e.preventDefault();
    e.dataTransfer.dropEffect = 'move';
    return false;
}

function dragDrop(e) {
    if (dragSrcEl != this) {
        dragSrcEl.innerHTML = this.innerHTML;
        this.innerHTML = e.dataTransfer.getData('text/html');
    }
    return false;
}

function dragEnd(e) {
    var listItems = document.querySelectorAll('.drag-container .draggable');
    [].forEach.call(listItems, function (item) {
        item.classList.remove('over');
    });
    this.style.opacity = '1';
}

function addEventsDragAndDrop(el) {
    removeEventsDragAndDrop(el);
    el.addEventListener('dragstart', dragStart, false);
    el.addEventListener('dragenter', dragEnter, false);
    el.addEventListener('dragover', dragOver, false);
    el.addEventListener('dragleave', dragLeave, false);
    el.addEventListener('drop', dragDrop, false);
    el.addEventListener('dragend', dragEnd, false);
}

function removeEventsDragAndDrop(el) {
    el.removeEventListener('dragstart', null);
    el.removeEventListener('dragenter', null);
    el.removeEventListener('dragover', null);
    el.removeEventListener('dragleave', null);
    el.removeEventListener('drop', null);
    el.removeEventListener('dragend', null);
}

