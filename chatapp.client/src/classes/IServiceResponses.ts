interface IServiceSuccessResponse<T> {
    success: true,
    data: T
};

interface IServiceFailureResponse<T> {
    success: false,
    data?: T
}

export type IServiceResponse<T> = IServiceSuccessResponse<T> | IServiceFailureResponse<T>;