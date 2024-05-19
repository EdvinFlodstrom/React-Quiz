import apiClient from "./apiClient";

const quizService = {
    initializeQuiz: (formData) => {
        return apiClient.post('/quiz/initialize', {
            playerName: formData.name,
            amountOfQuestions: formData.amountOfQuestions,
            questionType: formData.questionType,
        });
    },
    getQuestion: (playerName) => {
        return apiClient.post('/quiz/get', {
            playerName: playerName,
        });
    },
    checkAnswer: (formData) => {
        return apiClient.post('/quiz/answer', {
            playerName: formData.playerName,
            questionAnswer: formData.questionAnswer,
        });
    },
    createQuestion: (formData) => {
        return apiClient.post(`/quiz/create/${formData.questionType}`, {
            question: formData.question,
            option1: formData.option1,
            option2: formData.option2,
            option3: formData.option3,
            option4: formData.option4,
            correctOptionNumber: formData.correctOptionNumber,
        });
    },
    getQuestionById: (questionId) => {
        return apiClient.get(`/quiz/get/${questionId}`);
    },
    modifyQuestion: (questionId, formData) => {
        return apiClient.patch(`/quiz/patch/${questionId}`, {
            question: formData.question,
            option1: formData.option1,
            option2: formData.option2,
            option3: formData.option3,
            option4: formData.option4,
            correctOptionNumber: formData.correctOptionNumber,
        });
    },
    deleteQuestion: (questionId) => {
        return apiClient.delete(`/quiz/delete/${questionId}`);
    },
}

export default quizService;
