import { useState } from 'react';
import TakeQuiz from './TakeQuiz';

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
    const [errorMessage, setErrorMessage] = useState('');

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

    const handleFormSubmit = async (e) => {
        e.preventDefault();

        try {
            const response = await fetch(
                'https://localhost:7030/api/quiz/initialize',
                {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                    },
                    body: JSON.stringify({
                        playerName: formData.name,
                        amountOfQuestions: formData.numberOfQuestions,
                        questionType: formData.questionType,
                    }),
                }
            );

            if (response.ok) {
                adjustGradient();
                setQuizStarted(true);
            } else {
                const error = await response.text();
                setErrorMessage(error);
                console.error('Failed to initialize quiz:', error);
            }
        } catch (exception) {
            setErrorMessage(exception.message);
            console.error('Failed to initialize quiz:', exception.message);
        }
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
                    <p className='error-message'>{errorMessage}</p>
                </>
            ) : (
                <TakeQuiz />
            )}
        </>
    );
};

export default InitializeQuiz;
