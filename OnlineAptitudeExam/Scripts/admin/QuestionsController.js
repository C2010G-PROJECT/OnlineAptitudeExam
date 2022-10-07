const _QuestionsTable = '#questionsTable';
const _ACTION_Questions_Index = _PREFIX + '/Questions';
const _ACTION_Questions_GetData = _ACTION_Questions_Index + '/GetData';
const _ACTION_Questions_GetQuestion = _ACTION_Questions_Index + '/GetQuestion';
const _ACTION_Questions_Create = _ACTION_Questions_Index + '/Create';
const _ACTION_Questions_Update = _ACTION_Questions_Index + '/Update';
const _ACTION_Questions_Delete = _ACTION_Questions_Index + '/Delete';

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
    if (isCreate && !element.closest('.card').find('.data-card').data('can-create')) {
        showToast('This category has enough question!', 'warning');
        return;
    }

    // setup variable
    let mModal = $('#questionsModal'),
        mIcon = mModal.find('.modal-header i'),
        mTitle = mModal.find('.modal-title'),
        mSubmit = mModal.find('.modal-submit'),
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

    setupGenerateQuestion(mModal, type)

    // validate rule
    jQuery.validator.addMethod("anwer_unique", function (value, element) {
        let match = true;
        let questionsEle = $('.question-answer input[value!=""]');

        let addError = function (ele) {
            if (!ele.hasClass('error')) {
                ele.attr('aria-invalid', true)
                ele.removeClass('valid')
                ele.addClass('error');
                let label = ele.next('label');
                if (label.length == 0) {
                    let id = ele.attr('id');
                    label = $('<label class="error"></label>')
                    label.attr('id', id + '-error"')
                    label.attr('for', id)
                    ele.parent().append(label);
                }
                label.text('Answer is not unique.').css('display', 'block')
            }
        }

        let removeError = function (ele) {
            if (ele.hasClass('error')) {
                ele.attr('aria-invalid', false)
                ele.removeClass('error');
                ele.addClass('valid')
                ele.next('label').css('display', 'none')
            }
        }

        questionsEle.each(function () {
            removeError($(this))
            if (this != element && $(this).val() == value) {
                match = false;
            }
        })

        for (let i = 0; i < questionsEle.length - 1; i++) {
            for (let j = i + 1; j < questionsEle.length; j++) {
                var e1 = $(questionsEle[i]), e2 = $(questionsEle[j]);
                if (e1.val() === e2.val()) {
                    addError(e1)
                    addError(e2)
                }
            }
        }

        return match;
    }, "Answer is not unique.");

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

    // clear answer
    cancelSortAnswer(mModal);
    mContainerAnswer.html('')

    // bind data if edit
    let id = null;
    if (!isCreate) {
        id = element.closest('tr').data('id');
        loadUrl(_ACTION_Questions_GetQuestion, data => {
            mEdtQuestion.val(data.question);
            mSlbScore.find('option[value="' + data.score + '"]').prop('selected', true);
            let answers = JSON.parse(data.answers);
            let correct_answers = JSON.parse(data.correct_answers);
            answers.forEach(function (currentValue, index, arr) {
                mContainerAnswer.append(createAnwerElement(correct_answers.indexOf(index) != -1, currentValue))
            })
        }, null, 'POST', { id: id })
    }

    // init event
    mAddAnwer.off('click').on('click', () => mContainerAnswer.prepend(createAnwerElement()))
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
                    mContainerAnswer.prepend(createAnwerElement());
                }
                return;
            }

            if (correctAnswers.length == 0) {
                showToast('A question has at least one correct answer', 'warning')
                return;
            }

            let data = {
                id: id,
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
            e.preventDefault();
            e.stopPropagation();
        } else {
            mForm.find('.form-control.error')[0].focus()
        }
    })
    mModal.modal('show');
}

function setupGenerateQuestion(mModal, type) {
    if (type < 0 || type > 2) {
        return false;
    }
    var mForm = mModal.find('form'),
        mEdtQuestion = mModal.find('#Question'),
        mSlbScore = mForm.find('#Score'),
        mContainerAnswer = mModal.find('.container-answer'),
        mFooter = mModal.find('.modal-footer'),
        mRandGenerate = mFooter.find('.generate');
 
    mRandGenerate.off('click').on('click', function () {
        // clear data
        clearFormElements(mForm);
        cancelSortAnswer(mModal);
        mContainerAnswer.html('');

        var categoryName = type == 0 ? 'General Knowledge' : type == 1 ? 'Mathematics' : 'Computer Technology';
        var category = type == 0 ? 9 : type == 1 ? 19 : 18;
        var api = 'https://opentdb.com/api.php?encode=base64&amount=1&category=' + category;

        mRandGenerate.addClass('invisible')
        mForm.addClass('invisible')
        mForm.parent().append('<div class="loader" />')

        loadUrl(api, function (data) {
            mRandGenerate.removeClass('invisible')
            mForm.removeClass('invisible')
            mForm.next().remove();
            if (data.results.length > 0) {
                var qData = data.results[0]
                var difficulty = atob(qData.difficulty)
                mSlbScore.val(difficulty === 'easy' ? '1' : difficulty === 'medium' ? '2' : '3')
                mEdtQuestion.val(atob(qData.question));
                var answer = qData.incorrect_answers;
                answer.push(qData.correct_answer);
                answer = shuffleArray(answer);
                var cr_ans_index = answer.indexOf(qData.correct_answer)
                for (var i = 0; i < answer.length; i++) {
                    mContainerAnswer.append(createAnwerElement(i == cr_ans_index, atob(answer[i])))
                }
                showToast('Generated a ' + categoryName + ' question!', 'info', categoryName)
            } else {
                showToast('Generate failse!', 'warning', 'Failse')
            }
        }, function () {
            mRandGenerate.removeClass('invisible')
            mForm.removeClass('invisible')
            mForm.next().remove();
            showToast('Generate failse!', 'warning', 'Failse')
        })
    })
    return true;
}

function createAnwerElement(checked = false, value = '') {
    const random = (length = 8) => {
        return Math.random().toString(16).substr(2, length);
    };
    let name = random(14);
    let html =
        '<div class="d-flex align-items-center draggable item-answer">' +
        '<div class="me-3">' +
        '<input class="correct-answer" style="width: 18px; height: 18px " type="checkbox" title="Is correct answer" />' +
        '</div>' +
        '<div class="flex-grow-1 form-group question-answer">' +
        '<input id="' + name + '" name="' + name + '" class="form-control" placeholder="Answer" type="text" />' +
        '</div>' +
        '<div class="ms-2">' +
        '<button type="button" class="btn btn-danger btn-sm" onclick="removeAnwerElement($(this))">' +
        '<i class="mdi mdi-delete-outline"></i>' +
        '</button>' +
        '</div>' +
        '</div>';
    let ele = $(html);
    let answerInput = ele.find('.question-answer .form-control');
    answerInput.val(value)
    answerInput.attr('value', value)
    answerInput.on('focusout', function () {
        let val = $(this).val().trim();
        $(this).attr('value', val).val(val)
    })
    ele.find('.correct-answer').attr('checked', checked)

    var observer = new MutationObserver(function () {
        if (document.contains(ele[0])) {
            answerInput.rules('add', {
                onkeyup: false,
                required: true,
                maxlength: 200,
                anwer_unique: true
            })
            answerInput.focus();
            observer.disconnect();
        }
    });
    observer.observe(document, { attributes: false, childList: true, characterData: false, subtree: true });

    return ele;
}

function removeAnwerElement(ele) {
    ele.closest('.draggable').remove();
}

function toggleSortAnswer(ele) {
    let mModal = $('#questionsModal')
    let container = $('.drag-container');
    let mIcon = ele.find('i');
    let mSpan = ele.find('span');

    container.toggleClass('active')
    ele.toggleClass('btn-info btn-warning')
    mIcon.toggleClass('mdi-close mdi-arrow-all');
    mModal.find('.modal-submit, .add-answer, .generate').prop('disabled', (i, v) => !v);

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

function cancelSortAnswer(mModal) {
    let ele = mModal.find('.sort-answer');
    let container = $('.drag-container');
    let mIcon = ele.find('i');
    let mSpan = ele.find('span');

    container.removeClass('active')
    ele.removeClass('btn-warning').addClass('btn-info')
    mIcon.removeClass('mdi-close').addClass('mdi-arrow-all');
    $('#questionsModal').find('.modal-submit, .add-answer, .generate').prop('disabled', false);
    mSpan.text('Sort answer')
    container.find('.draggable').each(function () {
        removeEventsDragAndDrop(this);
        $(this).attr('draggable', false);
    });
}

function showDetailQuestionModal(element) {
    let mModal = $('#questionsDetailModal'),
        mScore = $('.score'),
        mQuestion = $('.question'),
        mContainer = mModal.find('.container-answer')

    let id = element.closest('tr').data('id');
    mContainer.html('')
    loadUrl(_ACTION_Questions_GetQuestion, data => {
        //console.log(data)
        mScore.text(data.score);
        mQuestion.text(data.question);
        answers = JSON.parse(data.answers);
        correctAnswers = JSON.parse(data.correct_answers);
        for (let i = 0; i < answers.length; i++) {
            let isCorrect = correctAnswers.indexOf(i) !== -1;
            let html =
                '<div class="d-flex">' +
                '<input class="me-3" type="checkbox" style="width:18px; pointer-events: none" ' + (isCorrect ? 'checked' : '') + '/>' +
                '<div class="p-3 my-2 flex-grow-1" style="border: #808080 dashed 1px">' + answers[i] + '</div>' +
                '</div>';
            mContainer.append(html)
        }
    }, null, 'POST', { id: id })
    mModal.modal('show')
}

function showDeleteQuestionModal(element, testId, type) {
    showConfirm('Delete question',
        'Are you sure to delete this record?',
        'outline-youtube',
        'delete-outline', () => QuestionsDelete(element, testId, type))
}

function loadTable(testId, type) {
    load(_ACTION_Questions_GetData, $(_QuestionsTable + type).closest('.card'), null, () => {
        let totalQuestion = 0;
        let totalQuestionEle = $('#totalQuestion');
        $('.card .data-card').each(function () {
            totalQuestion += $(this).data('total-question');
        })
        totalQuestionEle.text(totalQuestion + "/" + totalQuestionEle.data('limit'))
    }, 'POST', { testId: testId, type: type });
}

function QuestionsCreate(mModal, data, testId, type) {
    loadUrl(_ACTION_Questions_Create, data => {
        if (data.success) {
            mModal.modal('hide')
            showToast(data.message, data.msgType)
            loadTable(testId, type)
        } else {
            showToast(data.message, data.msgType)
        }
    }, null, 'POST', data)
}

function QuestionsUpdate(mModal, element, data) {
    loadUrl(_ACTION_Questions_Update, data => {
        if (data.success) {
            mModal.modal('hide')
            showToast(data.message, data.msgType)
            let tr = element.closest('tr');
            tr.find('.field-question').text(data.data.question)
            tr.find('.field-score').text(data.data.score)
        } else {
            showToast(data.message, data.msgType)
        }
    }, null, 'POST', data)
}

function QuestionsDelete(element, testId, type) {
    let id = element.closest('tr').data('id');
    loadUrl(_ACTION_Questions_Delete, data => {
        if (data.success) {
            showToast(data.message, data.msgType)
            loadTable(testId, type)
        } else {
            showToast(data.message, data.msgType)
        }
    }, null, 'POST', { id: id })
}
