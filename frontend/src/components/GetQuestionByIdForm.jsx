const GetQuestionByIdForm = ({
    handleGetQuestion,
    questionId,
    handleGetQuestionFormChange,
    getQuestionByIdButtonDisabled,
}) => {
    return (
        <form onSubmit={handleGetQuestion} className='form-container'>
            <label htmlFor='questionId' className='form-label'>
                ID of question to modify:
                <input
                    type='number'
                    name='questionId'
                    value={questionId}
                    onChange={handleGetQuestionFormChange}
                    className='form-input'
                />
            </label>
            <br />
            <button
                className='button form-button'
                disabled={getQuestionByIdButtonDisabled}>
                Get Question
            </button>
        </form>
    );
};

export default GetQuestionByIdForm;
