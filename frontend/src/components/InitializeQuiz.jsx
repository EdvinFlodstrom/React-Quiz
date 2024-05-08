const InitializeQuiz = ({ adjustGradient }) => {
    return (
        <>
            <h1>Take Quiz</h1>
            <h3>To start the quiz, please follow the instructions below...</h3>

            <button onClick={adjustGradient} className='button' disabled={true}>
                Initialize Quiz
            </button>
        </>
    );
};

export default InitializeQuiz;
