import { useState } from 'react';
import QuestionForm from './QuestionForm';

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

    function propertiesAreValid(...properties) {
        return properties.every(
            (property) =>
                property !== undefined && property !== null && property !== ''
        );
    }

    function formInputIsInvalid(updatedFormData) {
        if (
            !propertiesAreValid(
                updatedFormData.questionType,
                updatedFormData.question,
                updatedFormData.option1,
                updatedFormData.option2,
                updatedFormData.option3,
                updatedFormData.option4
            )
        )
            return true;

        if (
            updatedFormData.correctOptionNumber < 1 ||
            updatedFormData.correctOptionNumber > 4
        )
            return true;

        return false;
    }

    const handleFormSubmit = async (e) => {
        e.preventDefault();

        console.log(formData.correctOptionNumber === 2);

        try {
            const response = await fetch(
                `https://localhost:7030/api/quiz/create/${formData.questionType}`,
                {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                    },
                    body: JSON.stringify({
                        question: formData.question,
                        option1: formData.option1,
                        option2: formData.option2,
                        option3: formData.option3,
                        option4: formData.option4,
                        correctOptionNumber: formData.correctOptionNumber,
                    }),
                }
            );

            if (response.ok) {
                adjustGradient();
                setMessageAndState({
                    success: true,
                    message: 'Sucesss!',
                });
            } else {
                const error = await response.text();
                setMessageAndState({
                    success: false,
                    message: error,
                });
                console.error('Failed to create question:', error);
            }
        } catch (exception) {
            setMessageAndState({
                success: false,
                message: exception.message,
            });
            console.error('An error occured:', exception.message);
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
                createOrModifyQuestionButtonDisabled={
                    createQuestionButtonDisabled
                }
                handleFormSubmit={handleFormSubmit}
                correctOptionNumber={correctOptionNumber}
                isModifyingQuestion={false}
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
