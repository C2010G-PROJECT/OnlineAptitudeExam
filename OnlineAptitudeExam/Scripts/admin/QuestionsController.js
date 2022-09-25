const _QuestionsTable = '#questionsTable';
const _ACTION_Questions_Index = _PREFIX + '/Questions';
const _ACTION_Questions_GetData = _ACTION_Questions_Index + '/GetData';
const _ACTION_Questions_Create = _ACTION_Questions_Index + '/Create';

function toggleQuestionTable(element, tableId) {
    const mTable = $(tableId);
    mTable.toggleClass('mt-3')
    mTable.toggle(100, function () {
        let mIcon = element.find('i');
        mIcon.removeClass();
        if (mTable.is(':visible')) {
            mIcon.addClass('mdi mdi-minus-box-outline')
        } else {
            mIcon.addClass('mdi mdi-plus-box-outline')
        }
    });
}

function showQuestionsModal(element, testId, type, isCreate = true) {
    if (isCreate && element.closest('.card').find('.data-card').data('total-question') == 5) {
        showToast('This category has enough question!', 'warning');
        return;
    }

    // setup variable
    let mModal = $('#questionsModal'),
        mIcon = mModal.find('.modal-header i'),
        mTitle = mModal.find('.modal-title'),
        mSubmit = mModal.find('.modal-submit'),
        mSortAnwer = mModal.find('.sort-answer'),
        mAddAnwer = mModal.find('.add-answer'),
        mContainerAnswer = mModal.find('.container-answer'),
        mForm = mModal.find('form'),
        mEdtQuestion = mModal.find('#Question'),
        mSlbScore = mForm.find('#Score');
    // setup modal
    let title = isCreate ? 'Create' : 'Update';
    mIcon.removeClass().addClass('mdi mdi-' + (isCreate ? 'playlist-plus' : 'playlist-edit'))
    mTitle.text(title + ' question')
    mSubmit.text(title)

    // setup data
    pendingFocus(mModal, mEdtQuestion);
    clearFormElements(mForm);

    // validate rule
    mForm.validate({
        rules: {
            Question: {
                onkeyup: false,
                required: true,
                minlength: 10,
                maxlength: 300
            },
            Score: {
                required: true
            }
        },
        messages: {
            Question: {
                required: 'Please enter the question'
            },
            Score: {
                required: 'Please choose the score'
            }
        }
    }).resetForm()


    cancelSortAnswer(mSortAnwer);
    mContainerAnswer.html('')

    mAddAnwer.off('click').on('click', () => mContainerAnswer.prepend(createAnwer()))
    // init event
    mEdtQuestion.off('focusout').on('focusout', function () {
        $(this).val($(this).val().trim())
    })
    mSubmit.off('click').on('click', function (e) {
        if (mForm.valid()) {
            let question = mEdtQuestion.val();
            let score = mSlbScore.val();
            var answers = [], correctAnswers = [];

            mContainerAnswer.find('.question-answer input').each(function () {
                answers.push($(this).val())
            })

            mContainerAnswer.find('.correct-answer').each(function (i) {
                if ($(this).is(':checked')) {
                    correctAnswers.push(i)
                }
            })

            if (answers.length < 2) {
                showToast('A question has at least two answer.', 'warning');
                for (let i = 2; i > answers.length; i--) {
                    mContainerAnswer.prepend(createAnwer());
                }
                return;
            }

            if (correctAnswers.length == 0) {
                showToast('A question has at least one correct answer', 'warning')
                return;
            }

            let data = {
                id: 1,
                model: {
                    testId: testId,
                    type: type,
                    question: question,
                    answers: JSON.stringify(answers),
                    correctAnswers: JSON.stringify(correctAnswers),
                    score: score
                }
            }

            if (isCreate) {
                QuestionsCreate(mModal, data, testId, type);
            } else {
                QuestionsUpdate(mModal, element, data)
            }

            console.log(data)
            e.preventDefault();
            e.stopPropagation();
        } else {
            mForm.find('.form-control.error')[0].focus()
        }
    })
    mModal.modal('show');
}

function createAnwer(checked = false, value = '') {
    const random = (length = 8) => {
        return Math.random().toString(16).substr(2, length);
    };
    let name = random(14);
    let html = '<div class="d-flex align-items-center draggable item-answer">' +
        '<div class="me-3">' +
        '<input class="correct-answer" style="width: 18px; height: 18px " type="checkbox" title="Is correct answer" />' +
        '</div>' +
        '<div class="flex-grow-1 form-group question-answer">' +
        '<input id="' + name + '" name="' + name + '" class="form-control" placeholder="Answer" type="text" />' +
        '</div>' +
        '<div class="ms-2">' +
        '<button type="button" class="btn btn-danger btn-sm delete-answer">' +
        '<i class="mdi mdi-delete-outline"></i>' +
        '</button>' +
        '</div>' +
        '</div>';
    let ele = $(html);
    let answerInput = ele.find('.question-answer .form-control');
    answerInput.val(value)
    answerInput.on('focusout', function () {
        let val = $(this).val().trim();
        $(this).attr('value', val).val(val)
    })
    ele.find('.delete-answer').click(function () {
        $(this).closest('.draggable').remove();
    })
    ele.find('.correct-answer').prop('checked', checked)

    var observer = new MutationObserver(function () {
        if (document.contains(ele[0])) {
            answerInput.rules('add', {
                onkeyup: false,
                required: true,
                maxlength: 200
            })
            answerInput.focus();
            observer.disconnect();
        }
    });
    observer.observe(document, { attributes: false, childList: true, characterData: false, subtree: true });

    return ele;
}

function toggleSortAnswer(ele) {
    let mModal = $('#questionsModal')
    let container = $('.drag-container');
    let mIcon = ele.find('i');
    let mSpan = ele.find('span');

    container.toggleClass('active')
    ele.toggleClass('btn-info btn-warning')
    mIcon.toggleClass('mdi-close mdi-arrow-all');
    mModal.find('.modal-submit, .add-answer').prop('disabled', (i, v) => !v);

    if (container.hasClass('active')) {
        mSpan.text('Cancel')
        container.find('.draggable').each(function () {
            addEventsDragAndDrop(this);
            $(this).attr('draggable', true);
        });
    } else {
        mSpan.text('Sort answer')
        container.find('.draggable').each(function () {
            removeEventsDragAndDrop(this);
            $(this).attr('draggable', false);
        });
        container.find('.item-answer .question-answer input').each(function () {
            $(this).off('keyup').on('keyup', function () {
                $(this).attr('value', $(this).val().trim())
            })
        })
    }

}

function cancelSortAnswer(ele) {
    let container = $('.drag-container');
    let mIcon = ele.find('i');
    let mSpan = ele.find('span');

    container.removeClass('active')
    ele.removeClass('btn-warning').addClass('btn-info')
    mIcon.removeClass('mdi-close').addClass('mdi-arrow-all');
    $('#questionsModal').find('.modal-submit, .add-answer').prop('disabled', (i, v) => false);
    mSpan.text('Sort answer')
    container.find('.draggable').each(function () {
        removeEventsDragAndDrop(this);
        $(this).attr('draggable', false);
    });
}

function QuestionsCreate(mModal, data, testId, type) {
    loadUrl(_ACTION_Questions_Create, data => {
        console.log(data)
        if (data.success) {
            mModal.modal('hide')
            showToast(data.message, data.msgType)
            load(_ACTION_Questions_GetData, $(_QuestionsTable + type).closest('.card'), null, null, 'POST', { testId: testId, type: type });
        } else {
            showToast(data.message, data.msgType)
        }
    }, null, 'POST', data)
}

function QuestionsUpdate(mModal, element, data) {

}