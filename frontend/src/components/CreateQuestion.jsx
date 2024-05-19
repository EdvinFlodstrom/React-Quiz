import { useState } from 'react';
import { formInputIsInvalid } from '../utils/createOrModifyQuestionUtils';
import QuestionForm from './QuestionForm';
import quizService from '../services/quizService';

const CreateQuestion = ({ adjustGradient }) => {
    const [formData, setFormData] = useState({
        questionType: '',
        question: '',
        option1: '',
        option2: '',
        option3: '',
        option4: '',
        correctOptionNumber: 1,
    });
    const [createQuestionButtonDisabled, setCreateQuestionButtonDisabled] =
        useState(true);
    const [messageAndState, setMessageAndState] = useState({
        success: true,
        message: '',
    });
    const [correctOptionNumber, setCorrectOptionNumber] = useState(1);

    const handleFormChange = (e) => {
        const { name, value } = e.target;

        let newValue = value;
        if (name === 'correctOptionNumber') {
            newValue = Number(newValue);
            setCorrectOptionNumber(newValue);
        }
        const updatedFormData = { ...formData, [name]: newValue };

        setFormData(updatedFormData);
        setCreateQuestionButtonDisabled(formInputIsInvalid(updatedFormData));
    };

    const handleFormSubmit = async (e) => {
        e.preventDefault();

        try {
            await quizService.createQuestion(formData);

            adjustGradient();
            setMessageAndState({
                success: true,
                message: 'Sucesss!',
            });
        } catch (exception) {
            const message =
                exception.response.data || exception.response.statusText;

            setMessageAndState({
                success: false,
                message: message,
            });
            console.error('An error occured:', message);
        }
    };

    return (
        <>
            <h1>Create Question</h1>
            <h3>
                To create a question, please follow the instructions below...
            </h3>

            <QuestionForm
                formData={formData}
                handleFormChange={handleFormChange}
                submitFormButtonDisabled={createQuestionButtonDisabled}
                handleFormSubmit={handleFormSubmit}
                correctOptionNumber={correctOptionNumber}
                isModifyingQuestion={false}
                disableAllInput={false}
            />

            <p
                className={
                    messageAndState.success
                        ? 'success-message'
                        : 'error-message'
                }>
                {messageAndState.message}
            </p>
        </>
    );
};

export default CreateQuestion;
