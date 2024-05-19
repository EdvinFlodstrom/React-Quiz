import { useState } from 'react';
import TakeQuiz from './TakeQuiz';
import quizService from '../services/quizService';

const InitializeQuiz = ({ adjustGradient }) => {
    const [initializeQuizButtonDisabled, setInitializeQuizButtonDisabled] =
        useState(true);
    const [quizStarted, setQuizStarted] = useState(false);
    const [amountOfQuestions, setamountOfQuestions] = useState(2);
    const [formData, setFormData] = useState({
        name: '',
        amountOfQuestions: 2,
        questionType: '',
    });
    const [errorMessage, setErrorMessage] = useState('');

    const handleFormChange = (e) => {
        const { name, value } = e.target;

        if (name === 'amountOfQuestions') {
            setamountOfQuestions(value);
        }

        const updatedFormData = { ...formData, [name]: value };
        setFormData(updatedFormData);
        setInitializeQuizButtonDisabled(formInputIsInvalid(updatedFormData));
    };

    function formInputIsInvalid(updatedFormData) {
        const name = updatedFormData.name;
        const amountOfQuestions = updatedFormData.amountOfQuestions;

        if (name.length < 2 || name.length > 100) return true;
        if (amountOfQuestions < 2 || amountOfQuestions > 30) return true;

        return false;
    }

    const handleFormSubmit = async (e) => {
        e.preventDefault();

        try {
            await quizService.initializeQuiz(formData);

            adjustGradient();
            setQuizStarted(true);
        } catch (exception) {
            const message =
                exception.response.data || exception.response.statusText;

            setErrorMessage(message);
            console.error('An error occured:', message);
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
                            htmlFor='amountOfQuestions'
                            className='form-label'>
                            Number of Questions (2-30):
                            <input
                                type='number'
                                name='amountOfQuestions'
                                value={amountOfQuestions}
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
                            className='button form-button'
                            disabled={initializeQuizButtonDisabled}>
                            Initialize Quiz
                        </button>
                    </form>
                    <p className='error-message'>{errorMessage}</p>
                </>
            ) : (
                <TakeQuiz playerName={formData.name} />
            )}
        </>
    );
};

export default InitializeQuiz;
