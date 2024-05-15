const QuestionForm = ({
    formData,
    handleFormChange,
    submitFormButtonDisabled,
    handleFormSubmit,
    correctOptionNumber,
    isModifyingQuestion,
    disableAllInput,
}) => {
    return (
        <form onSubmit={handleFormSubmit} className='form-container'>
            <label htmlFor='questionType' className='form-label'>
                Question Type:
                <input
                    type='text'
                    name='questionType'
                    value={formData.questionType}
                    onChange={handleFormChange}
                    className='form-input'
                    disabled={isModifyingQuestion || disableAllInput}
                />
            </label>
            <br />
            <label htmlFor='name' className='form-label'>
                Question:
                <input
                    type='text'
                    name='question'
                    value={formData.question}
                    onChange={handleFormChange}
                    className='form-input'
                    disabled={disableAllInput}
                />
            </label>
            <br />
            <label htmlFor='option1' className='form-label'>
                Option 1:
                <input
                    type='text'
                    name='option1'
                    value={formData.option1}
                    onChange={handleFormChange}
                    className='form-input'
                    disabled={disableAllInput}
                />
            </label>
            <br />
            <label htmlFor='option2' className='form-label'>
                Option 2:
                <input
                    type='text'
                    name='option2'
                    value={formData.option2}
                    onChange={handleFormChange}
                    className='form-input'
                    disabled={disableAllInput}
                />
            </label>
            <br />
            <label htmlFor='option3' className='form-label'>
                Option 3:
                <input
                    type='text'
                    name='option3'
                    value={formData.option3}
                    onChange={handleFormChange}
                    className='form-input'
                    disabled={disableAllInput}
                />
            </label>
            <br />
            <label htmlFor='option4' className='form-label'>
                Option 4:
                <input
                    type='text'
                    name='option4'
                    value={formData.option4}
                    onChange={handleFormChange}
                    className='form-input'
                    disabled={disableAllInput}
                />
            </label>
            <br />
            <label htmlFor='correctOptionNumber' className='form-label'>
                Correct option number:
                <input
                    type='number'
                    name='correctOptionNumber'
                    value={correctOptionNumber}
                    onChange={handleFormChange}
                    className='form-input'
                    min='1'
                    max='4'
                    disabled={disableAllInput}
                />
            </label>
            <br />
            <button
                className='button form-button'
                disabled={submitFormButtonDisabled}>
                {disableAllInput
                    ? 'Delete Question'
                    : isModifyingQuestion
                    ? 'Update Question'
                    : 'Create Question'}
            </button>
        </form>
    );
};

export default QuestionForm;
