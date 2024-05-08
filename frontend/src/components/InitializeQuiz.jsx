import { useState } from 'react';

const InitializeQuiz = ({ adjustGradient }) => {
    const [initializeQuizButtonDisabled, setInitializeQuizButtonDisabled] =
        useState(true);
    const [quizStarted, setQuizStarted] = useState(false);
    const [numberOfQuestions, setNumberOfQuestions] = useState(2);
    const [formData, setFormData] = useState({
        name: '',
        numberOfQuestions: 2,
        questionType: '',
    });

    const handleFormChange = (e) => {
        const { name, value } = e.target;

        let updatedFormData;
        if (name === 'numberOfQuestions') {
            setNumberOfQuestions(value);
        }

        updatedFormData = { ...formData, [name]: value };
        setFormData(updatedFormData);
        setInitializeQuizButtonDisabled(FormInputIsInvalid(updatedFormData));
    };

    function FormInputIsInvalid(updatedFormData) {
        const name = updatedFormData.name;
        const numberOfQuestions = updatedFormData.numberOfQuestions;

        if (name.length < 2 || name.length > 100) return true;
        if (numberOfQuestions < 2 || numberOfQuestions > 30) return true;

        return false;
    }

    const handleFormSubmit = (e) => {
        e.preventDefault();
        adjustGradient();
        setQuizStarted(true);
    };

    return (
        <>
            {!quizStarted ? (
                <>
                    <h1>Take Quiz</h1>
                    <h3>
                        To start the quiz, please follow the instructions
                        below...
                    </h3>

                    <form
                        onSubmit={handleFormSubmit}
                        className='form-container'>
                        <label htmlFor='name' className='form-label'>
                            Name:
                            <input
                                type='text'
                                name='name'
                                value={formData.name}
                                onChange={handleFormChange}
                                className='form-input'
                                required
                            />
                        </label>
                        <br />
                        <label
                            htmlFor='numberOfQuestions'
                            className='form-label'>
                            Number of Questions (between 2 and 30):
                            <input
                                type='number'
                                name='numberOfQuestions'
                                value={numberOfQuestions}
                                onChange={handleFormChange}
                                className='form-input'
                                min='2'
                                max='30'
                                required
                            />
                        </label>
                        <br />
                        <label htmlFor='questionType' className='form-label'>
                            Question Type (optional):
                            <input
                                type='text'
                                name='questionType'
                                value={formData.questionType}
                                onChange={handleFormChange}
                                className='form-input'
                            />
                        </label>
                        <br />
                        <button
                            className='button'
                            disabled={initializeQuizButtonDisabled}>
                            Initialize Quiz
                        </button>
                    </form>
                </>
            ) : (
                <></>
            )}
        </>
    );
};

export default InitializeQuiz;
